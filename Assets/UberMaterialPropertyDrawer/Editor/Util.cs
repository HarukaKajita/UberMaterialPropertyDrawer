using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public static class Util
    {
        public static string GetPropertyTooltip(MaterialProperty prop)
        {
            return $"{prop.type} : {prop.name}";
        }
        public static int GetInt(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                return prop.intValue;
            else if(prop.type == MaterialProperty.PropType.Float)
                return (int)prop.floatValue;
            else if(prop.type == MaterialProperty.PropType.Range)
                return (int)prop.floatValue;
            Debug.LogError("Unsupported property type: " + prop.type);
            return 0;
        }
        public static void SetInt(MaterialProperty prop, int value)
        {
            if (prop.type == MaterialProperty.PropType.Int)
                prop.intValue = value;
            else if(prop.type == MaterialProperty.PropType.Float)
                prop.floatValue = value;
            else if(prop.type == MaterialProperty.PropType.Range)
                prop.floatValue = value;
            else
                Debug.LogError("Unsupported property type: " + prop.type);
        }
        
        public static bool GetAsBool(MaterialProperty prop)
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
        
        public static Object[] FetchSubAssets(Material mat)
        {
            var path = AssetDatabase.GetAssetPath(mat);
            return AssetDatabase.LoadAllAssetsAtPath(path);
        }
        
        public static Texture2D FetchSubAssetTexture(Material mat, string textureName)
        {
            var subAssets = FetchSubAssets(mat);
            return subAssets.OfType<Texture2D>().FirstOrDefault(a => a.name == textureName);
        }
        
        public static Texture2D[] FetchSubAssetTextureArray(Material[] mat, string textureName)
        {
            return mat.Select(e => FetchSubAssetTexture(e, textureName)).ToArray();
        }
        
        public static void DelaySaveAsset(Material mat)
        {
            EditorApplication.delayCall += () =>
            {
                var path = AssetDatabase.GetAssetPath(mat);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(path);
            };
        }
    }
}
