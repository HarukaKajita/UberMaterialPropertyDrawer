using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class CurveTextureDrawer : MaterialPropertyDrawer
    {
        private readonly string _groupName = "";
        
        private int _channelNum = 1;
        private int _resolution = 256;
        private bool _accumulate = false;
        private bool _useHalfTexture = false;

        public CurveTextureDrawer(string groupName, string[] args)
        {
            this._groupName = groupName;
            if (args == null || args.Length == 0) return;
            foreach (var argStr in args)
            {
                if (argStr.StartsWith("ch"))
                    this._channelNum = int.Parse(argStr[2..]);
                else if (argStr.StartsWith("res"))
                    this._resolution = int.Parse(argStr[3..]);
                else if (argStr.StartsWith("bit"))
                    this._useHalfTexture = int.Parse(argStr[3..]) == 16;
                else if (argStr == "accum")
                    this._accumulate = true;
            }
        }
        
        private string CurveTexName(MaterialProperty prop) => prop.name + "_CurveTex";

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (!UberDrawer.GetGroupExpanded(_groupName))
                return -2;
            var textureHeight = GUIHelper.TexturePropertyHeight;
            var interval = 2f;
            return textureHeight + interval;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "CurveTexture used on non-texture property");
                return;
            }
            
            if (!UberDrawer.GetGroupExpanded(_groupName)) return;

            var mat = editor.target as Material;
            if (mat == null) return;

            var path = AssetDatabase.GetAssetPath(mat);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            var dataName = prop.name + "_CurveData";
            var data = subAssets.OfType<CurveTextureData>().FirstOrDefault(a => a.name == dataName);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CurveTextureData>();
                data.name = dataName;
                AssetDatabase.AddObjectToAsset(data, mat);
                AssetDatabase.ImportAsset(path);
                subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            }

            if (data.texture == null && prop.textureValue != null)
            {
                var texName = CurveTexName(prop);
                var subAssetTex = subAssets.OfType<Texture2D>().FirstOrDefault(a => a.name == texName);
                data.texture = subAssetTex;
                prop.textureValue = subAssetTex;
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(mat);
            }

            EditorGUI.BeginChangeCheck();
            
            //Label GUI
            var indentSize = GUIHelper.IndentWidth;
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, propName);
            
            var valueWidth = position.width - labelRect.width + indentSize*2;
            var valueX = labelRect.width;
            // curve GUI
            var curveRect = new Rect(valueX, position.y, valueWidth/4, GUIHelper.TexturePropertyHeight/4);
            
            data.curveR = EditorGUI.CurveField(curveRect, "", data.curveR);
            curveRect.y += curveRect.height;
            data.curveG = EditorGUI.CurveField(curveRect, "", data.curveG);
            curveRect.y += curveRect.height;
            data.curveB = EditorGUI.CurveField(curveRect, "", data.curveB);
            curveRect.y += curveRect.height;
            data.curveA = EditorGUI.CurveField(curveRect, "", data.curveA);
            curveRect.y += curveRect.height;
            
            // texture GUI
            EditorGUI.BeginDisabledGroup(true);
            var textureY = position.y;
            var textureWidth = GUIHelper.TexturePropertyHeight;
            var textureRect = new Rect(curveRect.xMax + 2, textureY, textureWidth, textureWidth);
            editor.TextureProperty(textureRect, prop, "", false);
            EditorGUI.EndDisabledGroup();
            
            // Tiling Offset GUI
            var tillingOffsetHeight = GUIHelper.TillingOffsetPropertyHeight;
            var tillingOffsetY = position.y + (curveRect.height*4 - tillingOffsetHeight)/2;
            var tillingOffsetX = textureRect.xMax+2;
            var width = valueWidth-curveRect.width-textureWidth;
            var tilingOffsetRect = new Rect(tillingOffsetX, tillingOffsetY, width, tillingOffsetHeight);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);
            tilingOffsetRect.y += tilingOffsetRect.height;
            tilingOffsetRect.y += 2;

            // When changed shader lab property attribute value.(ex: resolution, channel num, bit)
            var isChangedTextureSettings = IsChangedTextureSettings(data);
            
            if (EditorGUI.EndChangeCheck() || isChangedTextureSettings)
            {
                var tex = BakeTexture(data);
                tex.name = CurveTexName(prop);
                if (!subAssets.Contains(tex))
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
        }
        
        private TextureFormat PickCorrectTextureFormat()
        {
            var format = TextureFormat.RGBA32;
            if (_useHalfTexture)
            {
                if(_channelNum == 1) format = TextureFormat.RHalf;
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
        
        private bool IsChangedTextureSettings(CurveTextureData data)
        {
            var format = PickCorrectTextureFormat();
            return data.texture.width != _resolution || data.texture.height != 1 || data.texture.format != format;
        }

        private Texture2D BakeTexture(CurveTextureData data)
        {
            var format = PickCorrectTextureFormat();

            var tex = data.texture;
            if (tex == null)
                tex = new Texture2D(_resolution, 1, format, true, true);
            else if (tex.width != _resolution || tex.height != 1 || tex.format != format)
                tex.Reinitialize(_resolution, 1, format, true);

            float accR = 0, accG = 0, accB = 0, accA = 0;
            var colors = new Color[_resolution];
            for (int i = 0; i < _resolution; i++)
            {
                float t = (float)i / (_resolution - 1);
                float r = data.curveR.Evaluate(t);
                float g = data.curveG.Evaluate(t);
                float b = data.curveB.Evaluate(t);
                float a = data.curveA.Evaluate(t);
                if (_accumulate)
                {
                    accR += r; r = accR;
                    accG += g; g = accG;
                    accB += b; b = accB;
                    accA += a; a = accA;
                }
                if (!_useHalfTexture)
                {
                    r = Mathf.Clamp01(r);
                    g = Mathf.Clamp01(g);
                    b = Mathf.Clamp01(b);
                    a = Mathf.Clamp01(a);
                }
                colors[i] = new Color(r, g, b, a);
            }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }
    }
}
