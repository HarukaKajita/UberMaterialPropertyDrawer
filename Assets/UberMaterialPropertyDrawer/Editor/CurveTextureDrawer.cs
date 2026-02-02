using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public class CurveTextureDrawer : UberDrawerBase
    {
        private int _channelNum = 1;
        private int _resolution = 256;
        private bool _accumulate = false;
        private bool _useHalfTexture = false;

        public CurveTextureDrawer(string groupName, params string[] args) : base(groupName)
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
                else if (argStr == "accum") _accumulate = true;
            }
        }
        
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.TexturePropertyHeight, editor);
        }

        private string CurveTexName(string propName) => propName + "_CurveTex";
        private string CurveDataName(string propName) => propName + "_CurveData";
        
        private CurveTextureData FetchCurveTextureData(string propName, Material mat)
        {
            var subAssets = Util.FetchSubAssets(mat);
            var dataName = CurveDataName(propName);
            var data = subAssets.OfType<CurveTextureData>().FirstOrDefault(a => a.name == dataName);
            return data;
        }
        
        private CurveTextureData[] FetchCurveTextureDataArray(MaterialProperty prop)
        {
            var data = 
                prop.targets.
                    OfType<Material>().
                    Select(e => FetchCurveTextureData(prop.name, e));
            return data.ToArray();
        }

        /// <summary>
        /// Initialize sub assets if need.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns> true if sub assets are initialized. false if sub assets are not initialized.</returns>
        private bool InitSubAssetsIfNeed(MaterialProperty prop)
        {
            var initialized = true;
            var targets = prop.targets;
            foreach (var target in targets)
            {
                if (target is not Material mat)continue;
                var data = FetchCurveTextureData(prop.name, mat);
                var dataName = CurveDataName(prop.name);    
                if (data == null)
                {
                    initialized = false;
                    data = ScriptableObject.CreateInstance<CurveTextureData>();
                    data.name = dataName;
                    AssetDatabase.AddObjectToAsset(data, mat);
                    EditorUtility.SetDirty(data);
                    EditorUtility.SetDirty(mat);
                    Util.DelaySaveAsset(mat);
                }
                // Textureが初期化されているか確認。subAssetのテクスチャとCurveTextureData.textureが一致する状態にする
                var subAssetTex = Util.FetchSubAssetTexture(mat, CurveTexName(prop.name));
                if (data.texture == null || subAssetTex == null || data.texture != subAssetTex)
                {
                    initialized = false;
                    if (subAssetTex != null)
                    {
                        data.texture = subAssetTex;
                        mat.SetTexture(prop.name, subAssetTex);
                    }
                    else
                    {
                        data.BakeTexture(_resolution, _accumulate, _useHalfTexture, PickCorrectTextureFormat(), CurveTexName(prop.name));
                        mat.SetTexture(prop.name, data.texture);
                        AssetDatabase.AddObjectToAsset(data.texture, mat);
                        Util.DelaySaveAsset(mat);
                    }
                    EditorUtility.SetDirty(data);
                    EditorUtility.SetDirty(mat);
                }
            }
            return initialized;
        }

        /// <summary>
        /// Ensure texture consistency between material and curve texture data.
        /// MaterialのTexturePropertyとCurveTextureDataのTextureとMaterialのSubAssetのTextureが一致することを保証する。
        /// </summary>
        /// <param name="prop"></param>
        /// <returns> true if texture is consistent. false if texture is not consistent.</returns>
        private bool EnsureTextureConsistency(MaterialProperty prop)
        {
            var consistent = true;
            var targets = prop.targets;
            foreach (var target in targets)
            {
                if (target is not Material mat) continue;
                var data = FetchCurveTextureData(prop.name, mat);
                var materialTextureValue = mat.GetTexture(prop.name);
                var subAssetTex = Util.FetchSubAssetTexture(mat, CurveTexName(prop.name));
                if (data.texture != materialTextureValue)
                {
                    consistent = false;
                    data.texture = subAssetTex;
                    mat.SetTexture(prop.name, subAssetTex);
                    EditorUtility.SetDirty(data);
                    EditorUtility.SetDirty(mat);
                }
            }
            return consistent;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "CurveTexture used on non-texture property");
                return;
            }
            
            // アセットの保存タイミングの影響で初期化が完了するまで遅延がある
            // アセットが正常に初期化されるまでGUIを表示しない
            var initialized = InitSubAssetsIfNeed(prop);
            if (!initialized) return;
            var consistent = EnsureTextureConsistency(prop);
            if (!consistent) return;
            
            // CurveをUndo対象にするにはSerializedPropertyを使用する必要がある
            var dataArray = FetchCurveTextureDataArray(prop);
            var dataSo = new SerializedObject(dataArray.ToArray<Object>());
            dataSo.Update();
            var curveRSp = dataSo.FindProperty("curveR");
            var curveGSp = dataSo.FindProperty("curveG");
            var curveBSp = dataSo.FindProperty("curveB");
            var curveASp = dataSo.FindProperty("curveA");
                
            MaterialEditor.BeginProperty(position, prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
            // Label GUI
            var indentSize = GUIHelper.IndentWidth;
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, GUIHelper.SingleLineHeight);
            EditorGUI.LabelField(labelRect, propName);

            // Curve GUI
            // When EditorGUI.PropertyField is called, EditorGUI.EndChangeCheck() will return true.
            var valueWidth = position.width - labelRect.width + indentSize * 2;
            var valueX = labelRect.width;
            var curveRect = new Rect(valueX, position.y, valueWidth / 4, GUIHelper.TexturePropertyHeight / 4);
            EditorGUI.PropertyField(curveRect, curveRSp, GUIContent.none);
            curveRect.y += curveRect.height;
            EditorGUI.PropertyField(curveRect, curveGSp, GUIContent.none);
            curveRect.y += curveRect.height;
            EditorGUI.PropertyField(curveRect, curveBSp, GUIContent.none);
            curveRect.y += curveRect.height;
            EditorGUI.PropertyField(curveRect, curveASp, GUIContent.none);
            curveRect.y += curveRect.height;
            dataSo.ApplyModifiedProperties();

            // Texture GUI
            EditorGUI.BeginDisabledGroup(true);
            var textureY = position.y;
            var textureWidth = GUIHelper.TexturePropertyHeight;
            var textureRect = new Rect(curveRect.xMax + 2, textureY, textureWidth, textureWidth);
            editor.TextureProperty(textureRect, prop, "", false);
            EditorGUI.EndDisabledGroup();

            // Tiling Offset GUI
            var tillingOffsetHeight = GUIHelper.TillingOffsetPropertyHeight;
            var tillingOffsetY = position.y + (curveRect.height * 4 - tillingOffsetHeight) / 2;
            var tillingOffsetX = textureRect.xMax + 2;
            var width = valueWidth - curveRect.width - textureWidth;
            var tilingOffsetRect = new Rect(tillingOffsetX, tillingOffsetY, width, tillingOffsetHeight);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);
            tilingOffsetRect.y += tilingOffsetRect.height;
            tilingOffsetRect.y += 2;

            EditorGUI.showMixedValue = false;
            // When changed shader lab property attribute value.(ex: resolution, channel num, bit)
            var isChangedTextureSettings = IsInconsistentTextureSettings(dataArray);

            if (EditorGUI.EndChangeCheck() || isChangedTextureSettings)
            {
                var textures = dataArray.Select(e => e.texture).ToArray<Object>();
                // Register property change undo before bake texture.
                editor.RegisterPropertyChangeUndo(propName);
                Undo.RecordObjects(textures, "Bake Curve Texture");

                foreach (var target in prop.targets)
                {
                    if (target is not Material mat) continue;
                    var data = FetchCurveTextureData(prop.name, mat);
                    data.BakeTexture(_resolution, _accumulate, _useHalfTexture, PickCorrectTextureFormat(), CurveTexName(prop.name));
                    EditorUtility.SetDirty(data);
                    EditorUtility.SetDirty(data.texture);
                    EditorUtility.SetDirty(mat);    
                }
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
        
        private bool IsInconsistentTextureSettings(CurveTextureData[] dataArray)
        {
            foreach (var data in dataArray)
            {
                if (data.texture == null) return true;
                var format = PickCorrectTextureFormat();
                if (data.texture.width != _resolution || data.texture.height != 1 || data.texture.format != format)
                    return true;
            }
            return false;
        }
    }
}
