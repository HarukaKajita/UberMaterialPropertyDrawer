using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberToggleDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = "";

        public UberToggleDrawer(string groupName)
        {
            this._groupName = groupName;
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (UberDrawer.GetGroupExpanded(_groupName))
                return EditorGUIUtility.singleLineHeight;
            else
                return -2;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!UberDrawer.GetGroupExpanded(_groupName)) return;

            var propName = ObjectNames.NicifyVariableName(label.text);
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var toggleValue = EditorGUI.Toggle(position, propName, prop.floatValue == 1);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                if(prop.type == MaterialProperty.PropType.Int)
                    prop.intValue = toggleValue ? 1 : 0;
                else if(prop.type == MaterialProperty.PropType.Float)
                    prop.floatValue = toggleValue ? 1 : 0;
            }
                
        }
    }
}
