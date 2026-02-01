using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class EndGroupDrawer : UberDrawerBase
    {
        public EndGroupDrawer(string groupName) : base(groupName)
        {
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            var data = GetGroupData(editor);
            if (TryEndGroup(data, prop))
                EndGroupScope(editor, GroupName);
            return GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            if (TryEndGroup(data, prop))
                EndGroupScope(editor, GroupName);
            EndPanel();
        }

        private void EndPanel()
        {
            EditorGUI.indentLevel -= 1;
        }

        private static bool TryEndGroup(GroupData data, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPop(data, prop?.name);
        }
    }
}
