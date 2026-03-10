using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// Helper class that repaints all Inspector windows
    /// 複数のInspectorを全て再描画するヘルパークラス
    /// </summary>
    public static class InspectorRepainter
    {
        public static void RepaintAllInspector()
        {
            var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var editorWindow in editorWindows)
            {
                if(editorWindow.titleContent.text == "Inspector")
                    editorWindow.Repaint();
            }
        }
    }
}
