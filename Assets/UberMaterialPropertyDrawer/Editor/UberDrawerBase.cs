using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public abstract class UberDrawerBase : MaterialPropertyDrawer
    {
        protected UberDrawerBase()
        {
            GroupName = string.Empty;
        }
        protected UberDrawerBase(string groupName)
        {
            GroupName = groupName ?? string.Empty;
            if (GroupName.Contains("/"))
            {
                UberDrawerLogger.LogError($"Group name cannot contain '/'. Use ':' instead. Group name: {groupName}");
                GroupName = GroupName.Replace("/", ":");
            }
        }

        protected string GroupName { get; }

        protected GroupData GetGroupData(MaterialEditor editor)
        {
            var mat = GetTargetMaterial(editor);
            if (mat == null) return null;
            return GroupDataCache.GetOrCreate(mat.shader);
        }
        
        public static Material GetTargetMaterial(MaterialEditor editor)
        {
            if (editor == null) return null;
            if (editor.target is Material mat) return mat;

            var targets = editor.targets;
            return targets?.FirstOrDefault(target => target is Material) as Material;
        }
        
        protected bool IsVisibleInGroup(MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            return GroupVisibility.CanShowContent(data, editor, GroupName);
        }

        protected float GetVisibleHeight(float visibleHeight, MaterialEditor editor)
        {
            return IsVisibleInGroup(editor) ? visibleHeight : GUIHelper.ClosedHeight;
        }

        protected void BeginGroupScope(MaterialEditor editor)
        {
            var parentPath = UberGroupState.GetCurrentPath(editor);
            var groupPath = UberGroupState.BuildPath(parentPath, GroupName);
            UberGroupState.PushPath(editor, groupPath);
        }

        protected void EndGroupScope(MaterialEditor editor, string expectedPath)
        {
            var poppedPath = UberGroupState.PopPath(editor);
            if (poppedPath != expectedPath)
                UberDrawerLogger.LogError($"Expected path '{expectedPath}', but popped '{poppedPath}'");
        }
    }
}
