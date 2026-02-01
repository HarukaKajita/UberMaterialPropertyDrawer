using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class Vector3Drawer : UberDrawerBase
    {
        public Vector3Drawer() : base()
        {
        }

        public Vector3Drawer(string groupName) : base(groupName)
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.SingleLineHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;

            var propName = ObjectNames.NicifyVariableName(label.text);
            var totalIndentSize = EditorGUI.indentLevel * GUIHelper.IndentWidth;
            var labelWidth = position.width * 0.3f;
            var valueWidth = position.width - labelWidth + totalIndentSize;
            var tmp_labelWidth = EditorGUIUtility.labelWidth;
            var tmp_fieldWidth = EditorGUIUtility.fieldWidth;
            var valueX = position.x + labelWidth - totalIndentSize;

            var labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            var valueRect = new Rect(valueX, position.y, valueWidth, position.height);
            EditorGUI.LabelField(labelRect, propName);
            prop.vectorValue = EditorGUI.Vector3Field(valueRect, GUIContent.none, prop.vectorValue);
            EditorGUIUtility.labelWidth = tmp_labelWidth;
            EditorGUIUtility.fieldWidth = tmp_fieldWidth;
            EditorGUIUtility.wideMode = false;
        }
    }
}
