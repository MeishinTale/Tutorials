using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CursorHandler
{
    public enum CursorType
    {
        Default = 0,
        HoverUI = 1,
        InteractItem = 2,
        InteractPNJ = 3
    }

    [System.Serializable]
    public class CustomCursor
    {
        public CursorType Name;
        public Vector2 Anchorage;   // Position the sprite
        public Texture2D Sprite;    // The Cursor texture
        public int Priority;        // Top Priority : 0, Low Priority +inf
        public CursorLockMode LockMode = CursorLockMode.None;

        public CustomCursor(CursorType name, Vector2 anchorage, Texture2D sprite, int priority, CursorLockMode lockMode)
        {
            Name = name;
            Anchorage = anchorage;
            Sprite = sprite;
            Priority = priority;
            LockMode = lockMode;
        }
    }

    private static Dictionary<CursorType, CustomCursor> cursorsDef = new Dictionary<CursorType, CustomCursor>();
    private static Dictionary<CustomCursor, List<object>> requests = new Dictionary<CustomCursor, List<object>>();
    private static List<CustomCursor> priorities = new List<CustomCursor>();

    public static Texture2D GetCurrentCursorTexture => curCursor.Sprite;
    public static Vector2 GetCurrentCursorAnchorage => curCursor.Anchorage;

    public static void InitCursors(List<CustomCursor> customCursors)
    {
        // Initialize Cursor Definition Dictionary
        customCursors.ForEach(x => cursorsDef.Add(x.Name, x));
        // Initialize Requests Dictionary
        customCursors.ForEach(x => requests.Add(x, new List<object>()));
        // Initialize Priorities list
        priorities = customCursors.OrderBy(x => x.Priority).ToList();
    }

    public static void SwitchCursors(List<CustomCursor> customCursos)
    {
        // Clean everything
        cursorsDef.Clear();
        requests.Clear();
        priorities.Clear();

        InitCursors(customCursos);
    }

    private static CustomCursor curCursor;

    public static void RequestCursor(object caller, CursorType cursorType)
    {
        // We want to check the list of requests for that particular CursorType
        List<object> cursorRequests = requests[cursorsDef[cursorType]];

        Debug.Log(string.Format("Requesting cursor {0} (Priority : {1:N0})", cursorsDef[cursorType].Name, cursorsDef[cursorType].Priority));

        if (!cursorRequests.Contains(caller))
        {
            // If not already requested by caller, request it
            cursorRequests.Add(caller);
            // Check if we need to update cursor
            CustomCursor cursor = checkRequests();
            if (cursor != curCursor)
                SetCursor(cursor);
        }
    }

    public static void UnRequestCursor(object caller, CursorType cursorType)
    {
        // Check the list of requests for that particular CursorType
        List<object> cursorRequests = requests[cursorsDef[cursorType]];

        Debug.Log(string.Format("<color=red>Un-Requesting</color> cursor {0} (Priority : {1:N0})", cursorsDef[cursorType].Name, cursorsDef[cursorType].Priority));

        if (cursorRequests.Contains(caller))
        {
            // Remove Request from List (For that caller)
            cursorRequests.Remove(caller);
            // Check if we need to update the current cursor
            CustomCursor cursor = checkRequests();
            if (cursor != curCursor)
                SetCursor(cursor);
        }
    }
    
    private static CustomCursor checkRequests()
    {
        // We simply loop on each cursor by priority order and return the first with requests
        foreach(CustomCursor cursor in priorities)
        {
            if (requests[cursor].Count > 0)
                return cursor;
        }
        // At that point no cursors were requested : Return default
        return cursorsDef[CursorType.Default];
    }

    private static void SetCursor(CustomCursor cursor)
    {
        // Positionate new Cursor Texture
        Vector2 hotspot = new Vector2(cursor.Sprite.width * cursor.Anchorage.x, cursor.Sprite.height * cursor.Anchorage.y);
        Cursor.lockState = cursor.LockMode;
        Cursor.SetCursor(cursor.Sprite, hotspot, CursorMode.Auto);
        // When locked, the cursor is placed in the center of the view and cannot be moved
        // The cursor is invisible in this state regardless of the value of Cursor.visible -> Add an OnGui on player Camera
        Cursor.visible = cursor.LockMode == CursorLockMode.Locked ? false : true;
        curCursor = cursor;
    }
}
