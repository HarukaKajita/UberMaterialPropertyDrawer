using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class GradientTextureDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = "";

        private int _channelNum = 1;
        private int _resolution = 256;
        private bool _half = false;

        public GradientTextureDrawer(string groupName, string[] args)
        {
            this._groupName = groupName;
            if (args == null || args.Length == 0) return;
            foreach (var argStr in args)
            {
                if (argStr.StartsWith("ch"))
                {
                    this._channelNum = int.Parse(argStr[2..]);
                }
                else if (argStr.StartsWith("res"))
                {
                    this._resolution = int.Parse(argStr[3..]);
                }
                else if (argStr.StartsWith("bit"))
                {
                    this._half = int.Parse(argStr[3..]) == 16;
                }
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!UberDrawer.GetGroupExpanded(_groupName))
                return -2;
            var gradientHeight = EditorGUIUtility.singleLineHeight;
            var textureHeight = EditorGUIUtility.singleLineHeight * 3.8f;
            return gradientHeight + textureHeight + 4;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "GradientTexture used on non-texture property");
                return;
            }
            
            if (!UberDrawer.GetGroupExpanded(_groupName)) return;

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
            if (tex == null || tex.width != _resolution || tex.format != format)
            {
                tex = new Texture2D(_resolution, 1, format, false, true);
            }

            var colors = new Color[_resolution];
            for (int i = 0; i < _resolution; i++)
            {
                float t = (float)i / (_resolution - 1);
                colors[i] = data.gradient.Evaluate(t);
            }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }
    }
}
