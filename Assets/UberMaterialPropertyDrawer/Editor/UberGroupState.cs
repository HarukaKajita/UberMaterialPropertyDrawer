using System.Collections.Generic;
using System.Linq;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal static class UberGroupState
    {
        private static readonly Dictionary<string, bool> GroupExpanded = new();
        private static readonly Stack<string> GroupNest = new();

        internal static bool GetGroupExpanded(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return true;
            if (GroupExpanded.TryGetValue(groupName, out var expanded))
            {
                UberDrawerLogger.Log("GetGroupExpanded : Existed " + groupName + " : " + expanded);
                return expanded;
            }

            var defaultValue = false;
            GroupExpanded.Add(groupName, defaultValue);
            UberDrawerLogger.Log("GetGroupExpanded : NOT Existed " + groupName + " : " + defaultValue);
            return defaultValue;
        }

        internal static void SetGroupExpanded(string groupName, bool state)
        {
            if (string.IsNullOrEmpty(groupName)) return;
            GroupExpanded[groupName] = state;
        }

        internal static void PushGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return;
            UberDrawerLogger.Log("Push : " + groupName);
            GroupNest.Push(groupName);
        }

        internal static string PopGroup()
        {
            if (GroupNest.Count == 0)
            {
                UberDrawerLogger.LogWarning("Pop called on empty group stack.");
                return string.Empty;
            }

            var popGroup = GroupNest.Pop();
            UberDrawerLogger.Log("Pop  : " + popGroup);
            return popGroup;
        }

        internal static bool TryPeekGroup(out string groupName)
        {
            if (GroupNest.Count == 0)
            {
                groupName = string.Empty;
                return false;
            }

            groupName = GroupNest.Peek();
            return true;
        }

        internal static int GetIndentLevel()
        {
            return GroupNest.Count;
        }

        internal static void ResetAll()
        {
            GroupExpanded.Clear();
            GroupNest.Clear();
        }

        internal static bool ParentGroupIsFolded(int indentNum)
        {
            UberDrawerLogger.Log("indentNum : " + indentNum);
            UberDrawerLogger.Log("GroupNest : " + GroupNest.Count);
            var groupArray = GroupNest.Reverse().ToArray();
            if (GroupNest.Count < indentNum) return false;
            UberDrawerLogger.Log("Parents : " + string.Join(", ", groupArray));
            for (var i = 0; i < indentNum; i++)
            {
                var parentalGroup = groupArray[i];
                UberDrawerLogger.Log("Parent " + parentalGroup + " -> " + (GetGroupExpanded(parentalGroup) ? "expanded" : "folded"));
                if (!GetGroupExpanded(parentalGroup)) return true;
            }
            return false;
        }
    }
}
