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
            
            var curveHeight = EditorGUIUtility.singleLineHeight * 4f;
            var textureHeight = Constants.TexturePropertyHeight;
            var tilingOffsetHeight = EditorGUIUtility.singleLineHeight*2f+2;
            var interval = 12f;
            return curveHeight + textureHeight + tilingOffsetHeight + interval;
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
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            // curve GUI
            data.curveR = EditorGUI.CurveField(line, "R", data.curveR);
            line.y += line.height;
            data.curveG = EditorGUI.CurveField(line, "G", data.curveG);
            line.y += line.height;
            data.curveB = EditorGUI.CurveField(line, "B", data.curveB);
            line.y += line.height;
            data.curveA = EditorGUI.CurveField(line, "A", data.curveA);
            line.y += line.height;

            // texture GUI
            EditorGUI.BeginDisabledGroup(true);
            editor.TextureProperty(line, prop, label.text, false);
            EditorGUI.EndDisabledGroup();
            line.y += Constants.TexturePropertyHeight;
            line.y += 2;
            
            // Tiling Offset GUI
            var totalIndentSize = EditorGUI.indentLevel * Constants.IndentWidth;
            var x = line.x + totalIndentSize;
            var width = line.width - totalIndentSize;
            var tilingOffsetRect = new Rect(x, line.y, width, EditorGUIUtility.singleLineHeight*2+2);
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
