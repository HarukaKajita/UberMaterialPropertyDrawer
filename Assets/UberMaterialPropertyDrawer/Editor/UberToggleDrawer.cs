using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class UberToggleDrawer : UberDrawerBase
    {
        public UberToggleDrawer() : base()
        {
        }

        public UberToggleDrawer(string groupName) : base(groupName)
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return IsVisibleInGroup() ? EditorGUIUtility.singleLineHeight : -2;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup()) return;

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
