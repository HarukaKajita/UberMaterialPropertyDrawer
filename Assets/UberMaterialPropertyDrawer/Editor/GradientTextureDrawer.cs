using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class GradientTextureDrawer : UberDrawerBase
    {
        private int _channelNum = 1;
        private int _resolution = 256;
        private bool _useHalfTexture = false;

        public GradientTextureDrawer(string groupName, params string[] args) : base(groupName)
        {
            ParseArgs(args);
        }

        private void ParseArgs(string[] args)
        {
            if (args == null || args.Length == 0) return;
            foreach (var argStr in args)
            {
                if (argStr.StartsWith("ch")) _channelNum = int.Parse(argStr[2..]);
                else if (argStr.StartsWith("res")) _resolution = int.Parse(argStr[3..]);
                else if (argStr.StartsWith("bit")) _useHalfTexture = int.Parse(argStr[3..]) == 16;
            }
        }

        private string GradientTexName(MaterialProperty prop) => prop.name + "_GradientTex";

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.TexturePropertyHeight, editor);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "GradientTexture used on non-texture property");
                return;
            }

            if (!IsVisibleInGroup(editor)) return;

            var mat = GetTargetMaterial(editor);
            if (mat == null) return;

            var path = AssetDatabase.GetAssetPath(mat);
            var subAssetsInMat = AssetDatabase.LoadAllAssetsAtPath(path);
            var dataName = prop.name + "_GradientData";
            var data = subAssetsInMat.OfType<GradientTextureData>().FirstOrDefault(a => a.name == dataName);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<GradientTextureData>();
                data.name = dataName;
                AssetDatabase.AddObjectToAsset(data, mat);
                data.BakeTexture(_resolution, PickCorrectTextureFormat(), GradientTexName(prop));
                prop.textureValue = data.texture;
                AssetDatabase.AddObjectToAsset(data.texture, mat);
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
                EditorApplication.delayCall += () =>
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(path);
                };
                subAssetsInMat = AssetDatabase.LoadAllAssetsAtPath(path);
                // return;
            }

            if (data.texture == null && prop.textureValue != null)
            {
                var texName = GradientTexName(prop);
                var subAssetTex = subAssetsInMat.OfType<Texture2D>().FirstOrDefault(a => a.name == texName);
                data.texture = subAssetTex;
                prop.textureValue = subAssetTex;
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
            }

            EditorGUI.BeginChangeCheck();
            MaterialEditor.BeginProperty(position, prop);
            
            // Label GUI
            var indentSize = GUIHelper.IndentWidth;
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, GUIHelper.SingleLineHeight);
            EditorGUI.LabelField(labelRect, propName);

            var valueWidth = position.width - labelRect.width + indentSize * 2;
            var valueX = labelRect.width;

            // Gradient GUI
            var gradientRect = new Rect(valueX, position.y, valueWidth / 4, GUIHelper.SingleLineHeight);
            data.gradient = EditorGUI.GradientField(gradientRect, "", data.gradient);

            // Texture GUI
            var texturePropWidth = GUIHelper.TexturePropertyHeight;
            var textureRect = new Rect(gradientRect.xMax, position.y, texturePropWidth, texturePropWidth);
            EditorGUI.BeginDisabledGroup(true);
            editor.TextureProperty(textureRect, prop, "", false);
            EditorGUI.EndDisabledGroup();

            // Tiling Offset GUI
            var tillingOffsetHeight = GUIHelper.TillingOffsetPropertyHeight;
            var tillingOffsetY = position.y + (textureRect.height - tillingOffsetHeight) / 2;
            var tillingOffsetX = textureRect.xMax + 2;
            var width = valueWidth - gradientRect.width - textureRect.width;
            var tilingOffsetRect = new Rect(tillingOffsetX, tillingOffsetY, width, tillingOffsetHeight);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);

            // When changed shader lab property attribute value.(ex: resolution, channel num, bit)
            var isChangedTextureSettings = IsChangedTextureSettings(data);

            if (EditorGUI.EndChangeCheck() || isChangedTextureSettings)
            {
                data.BakeTexture(_resolution, PickCorrectTextureFormat(), GradientTexName(prop));
                var tex = data.texture;
                if (!subAssetsInMat.Contains(tex))
                    AssetDatabase.AddObjectToAsset(tex, mat);
                data.texture = tex;
                prop.textureValue = tex;
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(tex);
                EditorUtility.SetDirty(mat);
                // AssetDatabase.ImportAsset(path);
                // AssetDatabase.SaveAssets();
            }

            if (data.texture != null && prop.textureValue != data.texture)
            {
                prop.textureValue = data.texture;
                EditorUtility.SetDirty(mat);
            }
            MaterialEditor.EndProperty();
        }

        private TextureFormat PickCorrectTextureFormat()
        {
            var format = TextureFormat.RGBA32;
            if (_useHalfTexture)
            {
                if (_channelNum == 1) format = TextureFormat.RHalf;
                else if (_channelNum == 2) format = TextureFormat.RGHalf;
                else if (_channelNum == 3) format = TextureFormat.RGBAHalf;
                else if (_channelNum == 4) format = TextureFormat.RGBAHalf;
            }
            else
            {
                if (_channelNum == 1) format = TextureFormat.R8;
                else if (_channelNum == 2) format = TextureFormat.RG16;
                else if (_channelNum == 3) format = TextureFormat.RGB24;
                else if (_channelNum == 4) format = TextureFormat.RGBA32;
            }
            return format;
        }

        private bool IsChangedTextureSettings(GradientTextureData data)
        {
            if (data.texture == null) return true;
            var format = PickCorrectTextureFormat();
            return data.texture.width != _resolution || data.texture.height != 1 || data.texture.format != format;
        }
    }
}
