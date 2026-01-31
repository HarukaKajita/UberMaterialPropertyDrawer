using System;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public sealed class UberDrawerContext
    {
        public string GroupName { get; }
        public string DrawerKey { get; }
        public string[] Args { get; }
        public string[] EnumNames { get; }
        public float[] EnumValues { get; }
        public string ParentGroup { get; }
        public string GroupStack { get; }

        public UberDrawerContext(
            string groupName,
            string drawerKey,
            string[] args,
            string[] enumNames,
            float[] enumValues,
            string parentGroup,
            string groupStack)
        {
            GroupName = groupName ?? string.Empty;
            DrawerKey = drawerKey ?? string.Empty;
            Args = args ?? Array.Empty<string>();
            EnumNames = enumNames;
            EnumValues = enumValues;
            ParentGroup = parentGroup ?? string.Empty;
            GroupStack = groupStack ?? string.Empty;
        }
    }
}
