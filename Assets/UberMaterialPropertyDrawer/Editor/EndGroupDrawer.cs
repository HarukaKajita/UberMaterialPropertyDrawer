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
            var endGroup = TryEndGroup(editor, prop);
            if (endGroup) EndGroupScope(editor, UberGroupState.GetCurrentPath(editor));
         
            var data = GetGroupData(editor);
            var parentPath = UberGroupState.GetCurrentPath(editor);
            var parentVisible = UberGroupState.IsCurrentScopeVisible(data, editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).GetPropertyHeight()");
            UberDrawerLogger.Log($"\t{nameof(endGroup)}:{endGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            
            return GUIHelper.ClosedHeight;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            var endGroup = TryEndGroup(editor, prop);
            if (endGroup) EndGroupScope(editor, UberGroupState.GetCurrentPath(editor));
            
            var data = GetGroupData(editor);
            var parentPath = UberGroupState.GetCurrentPath(editor);
            var parentVisible = UberGroupState.IsCurrentScopeVisible(data, editor);
            
            UberDrawerLogger.Log($"{GetType().Name}({GroupName}).OnGUI()");
            UberDrawerLogger.Log($"\t{nameof(endGroup)}:{endGroup}");
            UberDrawerLogger.Log($"\t{nameof(parentPath)}:{parentPath}");
            UberDrawerLogger.Log($"\t{nameof(parentVisible)}:{parentVisible}");
            
            // End Panel
            if(parentVisible)
                EditorGUI.indentLevel--;
        }

        private static bool TryEndGroup(MaterialEditor editor, MaterialProperty prop)
        {
            return UberGroupState.TryRecordPop(editor, prop?.name);
        }
    }
}
