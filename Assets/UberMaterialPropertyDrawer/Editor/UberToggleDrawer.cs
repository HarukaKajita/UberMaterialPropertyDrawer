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

            var toggleValue = Util.ToBool(prop);
            
            toggleValue = EditorGUI.Toggle(position, propName, toggleValue);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
                Util.SetBool(prop, toggleValue);
        }
    }
}
