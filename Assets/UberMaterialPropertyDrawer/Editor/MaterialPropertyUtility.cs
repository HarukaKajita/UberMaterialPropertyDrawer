using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public static class MaterialPropertyUtility
    {
        public static string GetPropertyTooltip(MaterialProperty prop)
        {
            return $"{prop.type} : {prop.name}";
        }

        public static GUIContent MakeLabelWithTooltip(MaterialProperty prop)
        {
            return new GUIContent(prop.displayName, GetPropertyTooltip(prop));
        }

        public static GUIContent MakeLabelWithTooltip(string label, MaterialProperty prop)
        {
            return new GUIContent(label, GetPropertyTooltip(prop));
        }

        public static int GetInt(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                return prop.intValue;
            else if (prop.type == MaterialProperty.PropType.Float)
                return (int)prop.floatValue;
            else if (prop.type == MaterialProperty.PropType.Range)
                return (int)prop.floatValue;

            Debug.LogError($"Unsupported property type: {prop.type} ({prop.name}).");
            return 0;
        }

        public static void SetInt(MaterialProperty prop, int value)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                prop.intValue = value;
            else if (prop.type == MaterialProperty.PropType.Float)
                prop.floatValue = value;
            else if (prop.type == MaterialProperty.PropType.Range)
                prop.floatValue = value;
            else
                Debug.LogError($"Unsupported property type: {prop.type} ({prop.name}).");
        }

        public static bool GetAsBool(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                return prop.intValue != 0;
            else if (prop.type == MaterialProperty.PropType.Float)
                return prop.floatValue != 0;
            else if (prop.type == MaterialProperty.PropType.Range)
                return prop.floatValue != 0;

            Debug.LogError($"Unsupported property type: {prop.type} ({prop.name}).");
            return false;
        }

        public static void SetBool(MaterialProperty prop, bool value)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                prop.intValue = value ? 1 : 0;
            else if (prop.type == MaterialProperty.PropType.Float)
                prop.floatValue = value ? 1 : 0;
            else if (prop.type == MaterialProperty.PropType.Range)
                prop.floatValue = value ? 1 : 0;
            else
                Debug.LogError($"Unsupported property type: {prop.type} ({prop.name}).");
        }
    }
}