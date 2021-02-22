using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CursorHandler;

[CreateAssetMenu(menuName ="CursorHandler/Definition")]
public class CursorsDefinition : ScriptableObject
{
    public List<CustomCursor> customCursors = new List<CustomCursor>()
    {
        new CustomCursor(CursorType.Default, new Vector2(0f, 0f), null, 10, CursorLockMode.None),
        new CustomCursor(CursorType.HoverUI, new Vector2(0f, 0f), null, 1, CursorLockMode.None),
        new CustomCursor(CursorType.InteractItem, new Vector2(0f, 0f), null, 2, CursorLockMode.None),
        new CustomCursor(CursorType.InteractPNJ, new Vector2(0f, 0f), null, 3, CursorLockMode.None),
    };
}
