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
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.SingleLineHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;

            var propName = ObjectNames.NicifyVariableName(label.text);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            var toggleValue = Util.GetAsBool(prop);

            toggleValue = EditorGUI.Toggle(position, propName, toggleValue);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
                Util.SetBool(prop, toggleValue);
        }
    }
}
