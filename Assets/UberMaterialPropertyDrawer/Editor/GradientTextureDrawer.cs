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
            var propertyHeight = 0f;
            var interval = 0f;
            // gradientHeight
            propertyHeight += EditorGUIUtility.singleLineHeight;
            interval += 2;
            // textureHeight
            propertyHeight += GUIHelper.TexturePropertyHeight;
            interval += 2;
            // tilingOffsetHeight
            propertyHeight += EditorGUIUtility.singleLineHeight*2f+2;
            interval += 2;
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
            
            // Gradient GUI
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            data.gradient = EditorGUI.GradientField(line, label.text, data.gradient);
            line.y += EditorGUIUtility.singleLineHeight;
            line.y += 2;
            
            // Texture GUI
            EditorGUI.BeginDisabledGroup(true);
            var textureRect = new Rect(line.x, line.y, line.width, GUIHelper.TexturePropertyHeight);
            editor.TextureProperty(textureRect, prop, label.text, false);
            EditorGUI.EndDisabledGroup();
            // EditorGUI.DrawRect(textureRect, new Color(1, 1, 1, 0.2f));
            textureRect.y += textureRect.height;
            textureRect.y += 2;
            
            // Tiling Offset GUI
            var totalIndentSize = EditorGUI.indentLevel * GUIHelper.IndentWidth;
            var x = textureRect.x + totalIndentSize;
            var width = textureRect.width - totalIndentSize;
            var tilingOffsetRect = new Rect(x, textureRect.y, width, EditorGUIUtility.singleLineHeight*2+2);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);
            tilingOffsetRect.y += tilingOffsetRect.height;
            tilingOffsetRect.y += 2;

            if (EditorGUI.EndChangeCheck())
            {
                var tex = BakeTexture(data);
                tex.name = prop.name + "_GradientTex";
                if (!subAssetsInMat.Contains(tex))
                    AssetDatabase.AddObjectToAsset(tex, mat);
                data.texture = tex;
                prop.textureValue = tex;
                // AssetDatabase.ImportAsset(path);
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(tex);
                EditorUtility.SetDirty(mat);
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
