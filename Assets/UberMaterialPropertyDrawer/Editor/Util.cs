using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public static class Util
    {
        public static bool GetBool(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                return prop.intValue != 0;
            else if(prop.type == MaterialProperty.PropType.Float)
                return prop.floatValue != 0;
            else if(prop.type == MaterialProperty.PropType.Range)
                return prop.floatValue != 0;
            Debug.LogError("Unsupported property type: " + prop.type);
            return false;
        }

        public static void SetBool(MaterialProperty prop, bool value)
        {
            prop.intValue = value ? 1 : 0;
            prop.floatValue = value ? 1 : 0;
        }
    }
}
