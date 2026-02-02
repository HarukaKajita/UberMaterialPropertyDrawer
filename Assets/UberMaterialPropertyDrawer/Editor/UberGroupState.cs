using System;
using System.Linq;
using UnityEditor;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class UberGroupState
    {
        internal static void BeginPass(GroupData data)
        {
            if (data == null) return;
            data.CurrentPassId++;
            data.PushedInPass.Clear();
            data.PoppedInPass.Clear();
        }

        internal static bool TryRecordPush(GroupData data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key)) return false;
            return data.PushedInPass.Add(key);
        }

        internal static bool TryRecordPop(GroupData data, string key)
        {
            if (data == null || string.IsNullOrEmpty(key)) return false;
            return data.PoppedInPass.Add(key);
        }

        /// <summary>
        /// Returns whether the specified group is open.
        /// This does not consider parent groups and only reflects the group's own state.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        internal static bool GetGroupExpanded(GroupData data, string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return true;
            if (data == null) return true;
            if (data.GroupExpanded.TryGetValue(groupName, out var expanded))
            {
                UberDrawerLogger.Log("GetGroupExpanded : Existed " + groupName + " : " + (expanded ? "開いている" : "閉じている"));
                return expanded;
            }

            var defaultValue = false;
            data.GroupExpanded.Add(groupName, defaultValue);
            UberDrawerLogger.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + (defaultValue　?　"開いている"　:　"閉じている"));
            return defaultValue;
        }

        internal static void SetGroupExpanded(GroupData data, string groupName, bool state)
        {
            if (string.IsNullOrEmpty(groupName) || data == null) return;
            data.GroupExpanded[groupName] = state;
        }

        internal static void PushGroup(GroupData data, string groupName)
        {
            if (string.IsNullOrEmpty(groupName) || data == null) return;
            UberDrawerLogger.Log("Push : " + groupName);
            data.GroupNest.Push(groupName);
        }

        internal static string PopGroup(GroupData data)
        {
            if (data == null || data.GroupNest.Count == 0)
            {
                UberDrawerLogger.LogWarning("Pop called on empty group stack.");
                return string.Empty;
            }

            var popGroup = data.GroupNest.Pop();
            UberDrawerLogger.Log("Pop  : " + popGroup);
            return popGroup;
        }

        internal static bool TryPeekGroup(GroupData data, out string groupName)
        {
            if (data == null || data.GroupNest.Count == 0)
            {
                groupName = string.Empty;
                return false;
            }

            groupName = data.GroupNest.Peek();
            return true;
        }

        internal static int GetGroupIndentLevel(GroupData data)
        {
            return data == null ? 0 : Math.Max(0,data.GroupNest.Count-1);
        }

        internal static void ResetAll(GroupData data)
        {
            if (data == null) return;
            data.GroupExpanded.Clear();
            data.GroupNest.Clear();
            data.PushedInPass.Clear();
            data.PoppedInPass.Clear();
            data.CurrentPassId = 0;
        }
        
        internal static void ResetNest(GroupData data)
        {
            if (data == null) return;
            data.GroupNest.Clear();
        }
        
        internal static bool ParentGroupIsClosed(GroupData data, int indentNum)
        {
            if (data == null) return false;
            UberDrawerLogger.Log("indentNum : " + indentNum);
            UberDrawerLogger.Log("GroupNest : " + data.GroupNest.Count);
            var groupArray = data.GroupNest.Reverse().ToArray();
            if (data.GroupNest.Count < indentNum) return false;
            UberDrawerLogger.Log("Parents : " + string.Join(", ", groupArray));
            for (var i = 0; i < indentNum; i++)
            {
                var parentalGroup = groupArray[i];
                UberDrawerLogger.Log("Parent " + parentalGroup + " -> " + (GetGroupExpanded(data, parentalGroup) ? "opened" : "closed"));
                if (!GetGroupExpanded(data, parentalGroup)) return true;
            }
            return false;
        }
    }
}
