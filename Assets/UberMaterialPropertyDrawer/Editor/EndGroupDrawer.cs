using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class EndGroupDrawer : UberDrawerBase
    {
        public EndGroupDrawer(string groupName) : base(groupName)
        {
            EndGroupScope(groupName);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            EndPanel();
        }

        private void EndPanel()
        {
            EditorGUI.indentLevel -= 1;
        }
    }
}
