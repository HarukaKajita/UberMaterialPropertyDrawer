using System;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DrawerKeyAttribute : Attribute
    {
        public string Key { get; }

        public DrawerKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
