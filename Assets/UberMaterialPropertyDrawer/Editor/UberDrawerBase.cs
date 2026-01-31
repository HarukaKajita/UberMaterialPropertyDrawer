using UnityEditor;

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
        protected int IndentLevel => UberGroupState.GetIndentLevel();

        protected bool IsVisibleInGroup()
        {
            if (string.IsNullOrEmpty(GroupName))
                return !UberGroupState.ParentGroupIsFolded(IndentLevel);

            return UberGroupState.GetGroupExpanded(GroupName)
                   && !UberGroupState.ParentGroupIsFolded(IndentLevel);
        }

        protected void BeginGroupScope()
        {
            UberGroupState.PushGroup(GroupName);
        }

        protected void EndGroupScope(string expectedGroup)
        {
            var actualGroup = UberGroupState.PopGroup();
            if (!string.IsNullOrEmpty(expectedGroup) && actualGroup != expectedGroup)
                UberDrawerLogger.LogError("Not Corresponded Group Begin-End : " + actualGroup + " - " + expectedGroup);
        }

        protected bool TryGetParentGroup(out string parentGroup)
        {
            return UberGroupState.TryPeekGroup(out parentGroup);
        }
    }
}
