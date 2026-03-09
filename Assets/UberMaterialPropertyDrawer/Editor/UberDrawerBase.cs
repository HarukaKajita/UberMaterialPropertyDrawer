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
        
        protected bool IsVisibleDrawer(MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            return GroupVisibility.CanShowContent(data, editor, GroupName);
        }

        protected float GetVisibleHeight(float visibleHeight, MaterialEditor editor)
        {
            return IsVisibleDrawer(editor) ? visibleHeight : GUIHelper.ClosedHeight;
        }

        protected void BeginGroupScope(MaterialEditor editor)
        {
            var parentPath = UberGroupState.GetCurrentPath(editor);
            var groupPath = UberGroupState.BuildPath(parentPath, GroupName);
            UberGroupState.PushPath(editor, groupPath);
        }

        protected void EndGroupScope(MaterialEditor editor)
        {
            var currentPath = UberGroupState.GetCurrentPath(editor);
            // check consistency within a declared group and current path
            var actualGroup = currentPath.Split("/").Last();
            var isValidPath = GroupName == actualGroup;
            if (!isValidPath) 
                UberDrawerLogger.LogWarning($"declared [EndGroup({GroupName})]. but actual group is '{actualGroup}'. Fix shader property attribute '[EndGroup({GroupName})]' to '[EndGroup({actualGroup})]'");
            
            var poppedPath = UberGroupState.PopPath(editor);
            if (poppedPath != currentPath)
                UberDrawerLogger.LogWarning($"Expected path '{currentPath}', but popped '{poppedPath}'");
        }
    }
}
