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
            var propertyHeight =GUIHelper.TexturePropertyHeight;
            var interval = 2;
            return propertyHeight + interval;
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
            var subAssetsInMat = AssetDatabase.LoadAllAssetsAtPath(path);
            var dataName = prop.name + "_GradientData";
            var data = subAssetsInMat.OfType<GradientTextureData>().FirstOrDefault(a => a.name == dataName);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<GradientTextureData>();
                data.name = dataName;
                AssetDatabase.AddObjectToAsset(data, mat);
                AssetDatabase.ImportAsset(path);
                subAssetsInMat = AssetDatabase.LoadAllAssetsAtPath(path);
            }

            EditorGUI.BeginChangeCheck();
            
            // Label GUI
            var indentSize = GUIHelper.IndentWidth;
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, propName);
            
            var valueWidth = position.width - labelRect.width + indentSize*2;
            var valueX = labelRect.width;
            
            // Gradient GUI
            var gradientRect = new Rect(valueX, position.y, valueWidth/4, EditorGUIUtility.singleLineHeight);
            data.gradient = EditorGUI.GradientField(gradientRect, "", data.gradient);
            
            // Texture GUI
            var texturePropWidth = GUIHelper.TexturePropertyHeight;
            var textureRect = new Rect(gradientRect.xMax, position.y, texturePropWidth, texturePropWidth);
            EditorGUI.BeginDisabledGroup(true);
            editor.TextureProperty(textureRect, prop, "", false);
            EditorGUI.EndDisabledGroup();
            
            // Tiling Offset GUI
            var tillingOffsetHeight = GUIHelper.TillingOffsetPropertyHeight;
            var tillingOffsetY = position.y + (textureRect.height - tillingOffsetHeight)/2;
            var tillingOffsetX = textureRect.xMax+2;
            var width = valueWidth - gradientRect.width - textureRect.width;
            var tilingOffsetRect = new Rect(tillingOffsetX, tillingOffsetY, width, tillingOffsetHeight);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);

            if (EditorGUI.EndChangeCheck())
            {
                var tex = BakeTexture(data);
                tex.name = prop.name + "_GradientTex";
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
                tex = new Texture2D(_resolution, 1, format, true, true);

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
