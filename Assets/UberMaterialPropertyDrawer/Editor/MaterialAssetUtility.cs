using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public static class MaterialAssetUtility
    {
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