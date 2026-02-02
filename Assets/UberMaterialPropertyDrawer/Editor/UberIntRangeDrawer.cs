using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberIntRangeDrawer : UberDrawerBase
    {
        private int min, max;
        public UberIntRangeDrawer(float min, float max) : base()
        {
            this.min = (int)min;
            this.max = (int)max;
        }

        public UberIntRangeDrawer(string groupName, float min, float max) : base(groupName)
        {
            this.min = (int)min;
            this.max = (int)max;
        }
        
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return IsVisibleInGroup(editor) ? GUIHelper.SingleLineHeight : GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;
            
            MaterialEditor.BeginProperty(position, prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
            var tmp = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width * 0.4f;
            
            var labelWithTooltip = new GUIContent(label.text, Util.GetPropertyTooltip(prop));
            var rect = EditorGUI.PrefixLabel(position, labelWithTooltip); ;
            var intValue = EditorGUI.IntSlider(rect, Util.GetInt(prop), min, max);
            
            EditorGUIUtility.labelWidth = tmp;
            
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                editor.RegisterPropertyChangeUndo(prop.name);
                Util.SetInt(prop, intValue);
            }
            MaterialEditor.EndProperty();
        }
    }
}
