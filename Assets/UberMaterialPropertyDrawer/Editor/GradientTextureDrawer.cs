using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class GradientTextureDrawer : MaterialPropertyDrawer
    {
        private int _size = 256;
        private bool _half = false;

        public GradientTextureDrawer() { }

        public GradientTextureDrawer(string args)
        {
            if (string.IsNullOrEmpty(args)) return;
            var tokens = args.Split(',');
            if (tokens.Length > 0) int.TryParse(tokens[0], out _size);
            if (tokens.Length > 1) _half = tokens[1].Trim().ToLowerInvariant().StartsWith("h");
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            const int lines = 2;
            return EditorGUIUtility.singleLineHeight * lines + 4;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "GradientTexture used on non-texture property");
                return;
            }

            var mat = editor.target as Material;
            if (mat == null) return;

            var path = AssetDatabase.GetAssetPath(mat);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            var dataName = prop.name + "_GradientData";
            var data = subAssets.OfType<GradientTextureData>().FirstOrDefault(a => a.name == dataName);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<GradientTextureData>();
                data.name = dataName;
                AssetDatabase.AddObjectToAsset(data, mat);
                AssetDatabase.ImportAsset(path);
                subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            }

            EditorGUI.BeginChangeCheck();
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            data.gradient = EditorGUI.GradientField(line, label.text, data.gradient);
            line.y += line.height + 2;

            EditorGUI.BeginDisabledGroup(true);
            editor.TextureProperty(line, prop, label.text, false);
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                var tex = BakeTexture(data);
                tex.name = prop.name + "_GradientTex";
                if (!subAssets.Contains(tex))
                    AssetDatabase.AddObjectToAsset(tex, mat);
                AssetDatabase.ImportAsset(path);
                data.texture = tex;
                prop.textureValue = tex;
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(tex);
                EditorUtility.SetDirty(mat);
                AssetDatabase.SaveAssets();
            }

            if (prop.textureValue != data.texture && data.texture != null)
            {
                AssetDatabase.RemoveObjectFromAsset(data.texture);
                Object.DestroyImmediate(data.texture, true);
                data.texture = null;
                EditorUtility.SetDirty(data);
                AssetDatabase.ImportAsset(path);
                AssetDatabase.SaveAssets();
            }
        }

        private Texture2D BakeTexture(GradientTextureData data)
        {
            var format = _half ? TextureFormat.RGBAHalf : TextureFormat.RGBA32;
            var tex = data.texture;
            if (tex == null || tex.width != _size || tex.format != format)
            {
                tex = new Texture2D(_size, 1, format, false, true);
            }

            var colors = new Color[_size];
            for (int i = 0; i < _size; i++)
            {
                float t = (float)i / (_size - 1);
                colors[i] = data.gradient.Evaluate(t);
            }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }
    }
}
