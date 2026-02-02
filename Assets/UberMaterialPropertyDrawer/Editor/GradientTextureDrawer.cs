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
        
        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(GUIHelper.TexturePropertyHeight, editor);
        }
        
        private string GradientTexName(string propName) => propName + "_GradientTex";
        private string GradientDataName(string propName) => propName + "_GradientData";
        
        private GradientData FetchGradientData(string propName, Material mat)
        {
            var subAssets = Util.FetchSubAssets(mat);
            var dataName = GradientDataName(propName);
            var data = subAssets.OfType<GradientData>().FirstOrDefault(a => a.name == dataName);
            return data;
        }
        
        private GradientData[] FetchGradientDataArray(MaterialProperty prop)
        {
            var data = 
                prop.targets.
                    OfType<Material>().
                    Select(e => FetchGradientData(prop.name, e));
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
                var data = FetchGradientData(prop.name, mat);
                var dataName = GradientDataName(prop.name);    
                if (data == null)
                {
                    initialized = false;
                    data = ScriptableObject.CreateInstance<GradientData>();
                    data.name = dataName;
                    AssetDatabase.AddObjectToAsset(data, mat);
                    EditorUtility.SetDirty(data);
                    EditorUtility.SetDirty(mat);
                    Util.DelaySaveAsset(mat);
                }
                // Textureが初期化されているか確認。subAssetのテクスチャとMaterialのテクスチャが一致する状態にする
                var subAssetTex = Util.FetchSubAssetTexture(mat, GradientTexName(prop.name));
                var materialTex = mat.GetTexture(prop.name);
                if (subAssetTex == null || subAssetTex != materialTex)
                {
                  initialized = false;
                  if (subAssetTex != null)
                  {
                      mat.SetTexture(prop.name, subAssetTex);
                  }
                  else
                  {
                      // subAssetTexが無い場合は、作ってサブアセット化
                      data.BakeTo(ref subAssetTex, _resolution, PickCorrectTextureFormat(), GradientTexName(prop.name));
                      mat.SetTexture(prop.name, subAssetTex);
                      AssetDatabase.AddObjectToAsset(subAssetTex, mat);
                      Util.DelaySaveAsset(mat);
                  }
                  EditorUtility.SetDirty(data);
                  EditorUtility.SetDirty(mat);
                } 
            }
            return initialized;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleInGroup(editor)) return;
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "GradientTexture used on non-texture property");
                return;
            }
            
            // アセットの保存タイミングの影響で初期化が完了するまで遅延がある
            // アセットが正常に初期化されるまでGUIを表示しない
            var initialized = InitSubAssetsIfNeed(prop);
            if (!initialized) return;
            
            // GradientをUndo対象にするにはSerializedPropertyを使用する必要がある
            var dataArray = FetchGradientDataArray(prop);
            var dataSo = new SerializedObject(dataArray.ToArray<Object>());
            dataSo.Update();
            var gradientSp = dataSo.FindProperty("gradient");
            
            MaterialEditor.BeginProperty(position, prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
            // Label GUI
            var indentSize = GUIHelper.IndentWidth;
            var propName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, GUIHelper.SingleLineHeight);
            EditorGUI.LabelField(labelRect, propName);

            var valueWidth = position.width - labelRect.width + indentSize * 2;
            var valueX = labelRect.width;

            // Gradient GUI
            // When EditorGUI.PropertyField is called, EditorGUI.EndChangeCheck() will return true.
            var gradientRect = new Rect(valueX, position.y, valueWidth / 4, GUIHelper.SingleLineHeight);
            EditorGUI.PropertyField(gradientRect, gradientSp, GUIContent.none);
            dataSo.ApplyModifiedProperties();
            
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

            EditorGUI.showMixedValue = false;
            
            // When changed shader lab property attribute value.(ex: resolution, channel num, bit)
            var materials = prop.targets.Cast<Material>().ToArray();
            var textures = Util.FetchSubAssetTextureArray(materials, GradientTexName(prop.name));
            var isChangedTextureSettings = IsInconsistentTextureSettings(textures);
            
            if (EditorGUI.EndChangeCheck() || isChangedTextureSettings)
            {
                // Register property change undo before bake texture.
                editor.RegisterPropertyChangeUndo(propName);
                Undo.RecordObjects(textures.ToArray<Object>(), "Bake Gradient Texture");

                for (var i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    var data = FetchGradientData(prop.name, mat);
                    var subAssetTex = textures[i];
                    data.BakeTo(ref subAssetTex, _resolution, PickCorrectTextureFormat(), GradientTexName(prop.name));
                    EditorUtility.SetDirty(data);
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

        private bool IsInconsistentTextureSettings(Texture2D[] textures)
        {
            foreach (var tex in textures)
            {
                var correctFormat = PickCorrectTextureFormat();
                if (tex.width != _resolution || tex.height != 1 || tex.format != correctFormat)
                    return true;
            }
            return false;
        }
    }
}
