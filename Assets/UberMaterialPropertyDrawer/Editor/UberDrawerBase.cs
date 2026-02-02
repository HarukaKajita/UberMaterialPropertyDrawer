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
        }

        protected string GroupName { get; }
        
        /// <summary>
        /// Returns whether the current group is visible.
        /// True only when all ancestor groups up to the direct parent are visible.
        /// </summary>
        /// <returns></returns>
        protected bool IsVisibleInGroup(MaterialEditor editor)
        {
            var data = GetGroupData(editor);
            if (data == null) return true;
            var indentLevel = UberGroupState.GetGroupIndentLevel(data);
            if (string.IsNullOrEmpty(GroupName))
                return !UberGroupState.ParentGroupIsClosed(data, indentLevel);

            return UberGroupState.GetGroupExpanded(data, GroupName)
                   && !UberGroupState.ParentGroupIsClosed(data, indentLevel);
        }

        protected float GetVisibleHeight(float visibleHeight, MaterialEditor editor)
        {
            return IsVisibleInGroup(editor) ? visibleHeight : GUIHelper.ClosedHeight;
        }

        protected void BeginGroupScope(MaterialEditor editor)
        {
            UberGroupState.PushGroup(GetGroupData(editor), GroupName);
        }

        protected void EndGroupScope(MaterialEditor editor, string expectedGroup)
        {
            var actualGroup = UberGroupState.PopGroup(GetGroupData(editor));
            if (!string.IsNullOrEmpty(expectedGroup) && actualGroup != expectedGroup)
                UberDrawerLogger.LogError("Not Corresponded Group Begin-End : " + actualGroup + " - " + expectedGroup);
        }

        protected string GetParentGroup(MaterialEditor editor, string groupName)
        {
            UberDrawerLogger.Log($"GetParentGroup : from {groupName}");
            // groupNameを考慮して親グループを取得する
            var groupData = GetGroupData(editor);
            var groupNest = groupData.GroupNest.ToArray();
            for (var i = 0; i < groupNest.Length; i++)
            {
                UberDrawerLogger.Log($"\t[{i}] : {groupNest[i]} : {groupName == groupNest[i]}");
                if (groupNest[i] != groupName) continue;
                if(i < groupNest.Length-1)
                {
                    UberDrawerLogger.Log($"\treturn {groupNest[i + 1]}");
                    return groupNest[i + 1];
                }
            }
            // when i==0 && groupNest[0] == groupName
            UberDrawerLogger.Log($"\treturn string.Empty");
            return string.Empty;
        }

        protected GroupData GetGroupData(MaterialEditor editor)
        {
            return GroupDataCache.GetOrCreate(GetTargetMaterial(editor));
        }

        internal static Material GetTargetMaterial(MaterialEditor editor)
        {
            if (editor == null) return null;
            if (editor.target is Material mat) return mat;

            var targets = editor.targets;
            if (targets == null) return null;
            foreach (var target in targets)
            {
                if (target is Material m) return m;
            }
            return null;
        }
    }
}
