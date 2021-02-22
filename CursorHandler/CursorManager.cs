using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static CursorHandler;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private CursorsDefinition bronzeCursors = default;
    [SerializeField] private CursorsDefinition silverCursors = default;

    // Start is called before the first frame update
    void Start()
    {
        // Cursors Initialization
        InitializeCursors(bronzeCursors);
        
        InitiateTest();
    }
    
    private void InitializeCursors(CursorsDefinition cursorsDefinition)
    {
        if (cursorsDefinition != null && cursorsDefinition.customCursors.Count > 0)
            InitCursors(cursorsDefinition.customCursors);
    }

    private void OnGUI()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 cursorPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        // Get Cursor Texture
        Texture2D cursorSprite = GetCurrentCursorTexture;
        Vector2 cursorAnchorage = GetCurrentCursorAnchorage;
        GUI.DrawTexture(new Rect(cursorPosition.x - (cursorSprite.width * cursorAnchorage.x),
                                cursorPosition.y - (cursorSprite.height * cursorAnchorage.y),
                                cursorSprite.width,
                                cursorSprite.height), cursorSprite);
        // Get Cursor Anchor
    }

    #region --- For Testing Only ---
    private void InitiateTest()
    {
        StartCoroutine(switchCursor(0, true));

        endOfTest += onEndOfBronzeTestrunSilverTest;
    }

    private void onEndOfBronzeTestrunSilverTest()
    {
        SwitchCursors(silverCursors.customCursors);
        InitiateTest();
    }

    private event Action endOfTest;
    IEnumerator switchCursor(int curEnum, bool requesting)
    {
        if (requesting)
            RequestCursor(this, (CursorType)curEnum);
        else
            UnRequestCursor(this, (CursorType)curEnum);


        yield return new WaitForSeconds(2f);


        if (curEnum < Enum.GetValues(typeof(CursorType)).Length - 1)
            StartCoroutine(switchCursor(curEnum + 1, requesting));
        else if (curEnum == Enum.GetValues(typeof(CursorType)).Length - 1 && requesting)
            StartCoroutine(switchCursor(0, !requesting));
        else
            endOfTest.Invoke();
    }

    #endregion
}
