using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class VectorDrawer : UberDrawerBase
    {
        private int _dimension;
        public VectorDrawer(float dim) : base()
        {
            _dimension = (int)dim;
        }

        public VectorDrawer(string groupName, float dim) : base(groupName)
        {
            _dimension = (int)dim;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.SingleLineHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;
            MaterialEditor.BeginProperty(position,prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
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
            var v = prop.vectorValue;
            if (_dimension == 2)
                v = EditorGUI.Vector2Field(valueRect, GUIContent.none, v);
            else if(_dimension == 3)
                v = EditorGUI.Vector3Field(valueRect, GUIContent.none, v);
            else if(_dimension == 4)
                v = EditorGUI.Vector4Field(valueRect, GUIContent.none, v);
            EditorGUIUtility.labelWidth = tmp_labelWidth;
            EditorGUIUtility.fieldWidth = tmp_fieldWidth;
            
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                editor.RegisterPropertyChangeUndo(propName);
                prop.vectorValue = v;
            }
            MaterialEditor.EndProperty();
        }
    }
}
