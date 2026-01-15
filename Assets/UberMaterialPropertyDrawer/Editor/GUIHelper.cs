using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class GUIHelper
    {
        internal static readonly float IndentWidth = 15f;
        internal static readonly float Interval = 2f;
        internal static readonly float SingleLineHeight = EditorGUIUtility.singleLineHeight;
        internal static readonly float TexturePropertyHeight = EditorGUIUtility.singleLineHeight *3.5f;

        internal static float GetIndentWidth() => EditorGUI.indentLevel * IndentWidth;
        internal static Rect Indent(Rect rect, bool shrinkWidth = false)
        {
            var x = rect.x + GetIndentWidth();
            var width = rect.width - (shrinkWidth ? GetIndentWidth() : 0);
            return new Rect(x, rect.y, width, rect.height);
        }
    }
}
