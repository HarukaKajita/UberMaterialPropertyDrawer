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
        private bool _half = false;

        public CurveTextureDrawer(string groupName, string[] args)
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
                }else if (argStr == "accum")
                {
                    this._accumulate = true;
                }
            }
        }

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

            EditorGUI.BeginChangeCheck();
            
            //Label GUI
            var indentSize = GUIHelper.IndentWidth;
            // EditorGUI.DrawRect(
            //     new Rect(GUIHelper.Indent(position, true)),
            //     new Color(1, 1, 1, 0.2f)
            //     );
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, propName);
            
            var valueWidth = position.width - labelRect.width + indentSize*2;
            var valueX = labelRect.width;
            // var valueRect = new Rect(valueX, position.y, valueWidth, position.height);
            // EditorGUI.DrawRect(EditorGUI.IndentedRect(labelRect), new Color(1, 0, 0, 0.2f));
            // EditorGUI.DrawRect(EditorGUI.IndentedRect(valueRect), new Color(0, 0, 1, 0.2f));
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

            if (EditorGUI.EndChangeCheck())
            {
                var tex = BakeTexture(data);
                tex.name = prop.name + "_CurveTex";
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

        private Texture2D BakeTexture(CurveTextureData data)
        {
            var format = _half ? TextureFormat.RGBAHalf : TextureFormat.RGBA32;
            
            var tex = data.texture;
            if (tex == null || tex.width != _resolution || tex.format != format)
            {
                tex = new Texture2D(_resolution, 1, format, false, true);
            }

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
                if (!_half)
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
