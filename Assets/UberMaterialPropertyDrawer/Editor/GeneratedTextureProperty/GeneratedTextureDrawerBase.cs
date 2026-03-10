using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    /// <summary>
    /// 何らかのデータをテクスチャ化してTexture Propertyに紐づけるPropertyDrawerのベースクラス。
    /// 共通フローを担当。
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TOption"></typeparam>
    public abstract class GeneratedTextureDrawerBase<TData, TOption> : UberDrawerBase where TData : GeneratedTextureDataBase
    {
        private readonly TOption _textureOptions;
        
        protected abstract TOption ParseOptions(string[] args);
        protected abstract string GetDataName(string propName);
        protected abstract string GetTextureName(string propName);
        protected abstract bool HasTextureOptionMismatch(Texture2D[] textures, TOption options);
        protected abstract void DrawDetailFields(Rect position, SerializedObject so);
        protected abstract void Bake(TData data, ref Texture2D tex, TOption options, string texName);

        protected GeneratedTextureDrawerBase(string groupName, params string[] args) : base(groupName)
        {
            _textureOptions = ParseOptions(args);
        }

        public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            if (!IsVisibleDrawer(editor)) return;
            if (prop.type != MaterialProperty.PropType.Texture)
            {
                EditorGUI.LabelField(position, "GeneratedTexture used on non-texture property");
                return;
            }

            var targets = editor.targets.Cast<Material>().ToArray();
            foreach (var target in targets)
            {
                GeneratedTextureCleanupService.CleanupMaterial(target, CleanupMode.Interactive);
            }

            var subAssetStore = new MaterialSubAssetStore<TData>();
            var bindings = subAssetStore.EnsureBindings(targets, prop.name, GetDataName(prop.name), GetTextureName(prop.name));
            
            var dataArray = bindings.Select(b => b.Data).ToArray();
            var dataSo = new SerializedObject(dataArray.ToArray<Object>());
            dataSo.Update();
            
            MaterialEditor.BeginProperty(position, prop);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            
            var indentSize = GUIHelper.IndentWidth;
            var displayPropName = ObjectNames.NicifyVariableName(label.text);
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, GUIHelper.SingleLineHeight);
            EditorGUI.LabelField(labelRect, displayPropName);
            
            var valueWidth = position.width - labelRect.width + indentSize * 2;
            var valueX = labelRect.width;
            var detailFieldsRect = new Rect(valueX, position.y, valueWidth / 4, GUIHelper.TexturePropertyHeight / 4);
            DrawDetailFields(detailFieldsRect, dataSo);
            
            EditorGUI.BeginDisabledGroup(true);
            var textureY = position.y;
            var textureWidth = GUIHelper.TexturePropertyHeight;
            var textureRect = new Rect(detailFieldsRect.xMax + 2, textureY, textureWidth, textureWidth);
            editor.TextureProperty(textureRect, prop, "", false);
            EditorGUI.EndDisabledGroup();

            var tillingOffsetHeight = GUIHelper.TillingOffsetPropertyHeight;
            var tillingOffsetY = position.y + (detailFieldsRect.height * 4 - tillingOffsetHeight) / 2;
            var tillingOffsetX = textureRect.xMax + 2;
            var width = valueWidth - detailFieldsRect.width - textureWidth;
            var tilingOffsetRect = new Rect(tillingOffsetX, tillingOffsetY, width, tillingOffsetHeight);
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);

            EditorGUI.showMixedValue = false;
            var textures = Util.FetchSubAssetTextureArray(targets, GetTextureName(prop.name));
            var isChangedTextureSettings = HasTextureOptionMismatch(textures, _textureOptions);

            if (EditorGUI.EndChangeCheck() || isChangedTextureSettings)
            {
                editor.RegisterPropertyChangeUndo(displayPropName);
                Undo.RecordObjects(textures.ToArray<Object>(), "Bake Curve Texture");
                
                foreach (var binding in bindings)
                {
                    var subAssetTex = binding.Texture;
                    Bake(binding.Data, ref subAssetTex, _textureOptions, GetTextureName(prop.name));
                    EditorUtility.SetDirty(binding.Data);
                    EditorUtility.SetDirty(binding.Texture);
                    EditorUtility.SetDirty(binding.Material);
                }
            }
            MaterialEditor.EndProperty();
        }

    }
}
