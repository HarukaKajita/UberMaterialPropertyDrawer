using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
        private const float ContentSpacing = 4f;
        private readonly TOption _textureOptions;

        protected abstract int DetailLineCount { get; }
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

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            UberDrawerLogger.Log($"GetPropertyHeight: {GetType().Name}");
            return GetVisibleHeight(CalculatePropertyHeight(), editor);
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
                GeneratedTextureCleanupService.CleanupMaterial(target, CleanupMode.Interactive);

            var propName = prop.name;
            var textureName = GetTextureName(propName);
            var dataName = GetDataName(propName);
            var subAssetStore = new MaterialSubAssetStore<TData>();
            var bindings = subAssetStore.EnsureBindings(targets, propName, dataName, textureName);
            if (bindings.Length == 0) return;

            var dataObjects = bindings.Select(binding => (Object)binding.Data).ToArray();
            var dataSo = new SerializedObject(dataObjects);
            dataSo.Update();

            MaterialEditor.BeginProperty(position, prop);

            var displayPropName = ObjectNames.NicifyVariableName(label.text);
            DrawLabel(position, displayPropName);

            var contentRect = GetContentRect(position);
            var topAreaHeight = Mathf.Max(GetDetailHeight(), GUIHelper.TexturePropertyHeight);
            var detailRect = GetDetailRect(contentRect);
            var previewRect = GetPreviewRect(contentRect, detailRect);
            var tilingOffsetRect = GetTilingOffsetRect(contentRect, previewRect);
            var settingsRect = GetSettingsRect(contentRect, topAreaHeight);

            EditorGUI.BeginChangeCheck();
            DrawDetailFields(detailRect, dataSo);
            var detailChanged = EditorGUI.EndChangeCheck();

            DrawTexturePreview(previewRect, prop, editor);
            DrawTilingOffset(tilingOffsetRect, prop, editor);

            EditorGUI.BeginChangeCheck();
            DrawTextureSettingsFields(settingsRect, dataSo);
            var settingsChanged = EditorGUI.EndChangeCheck();

            dataSo.ApplyModifiedProperties();

            var optionMismatch = HasTextureOptionMismatch(bindings.Select(binding => binding.Texture).ToArray(), _textureOptions);
            var samplerMismatch = HasTextureSettingMismatch(bindings);
            var colorSpaceMismatch = HasColorSpaceMismatch(bindings);
            var requiresSync = detailChanged || settingsChanged || optionMismatch || samplerMismatch || colorSpaceMismatch;
            var requiresBake = detailChanged || optionMismatch || colorSpaceMismatch;

            if (requiresSync)
                bindings = subAssetStore.EnsureBindings(targets, propName, dataName, textureName);

            if (requiresBake)
                BakeBindings(bindings, textureName, displayPropName, editor);

            MaterialEditor.EndProperty();
        }

        private void BakeBindings(GeneratedTextureBinding<TData>[] bindings, string textureName, string displayPropName, MaterialEditor editor)
        {
            editor.RegisterPropertyChangeUndo(displayPropName);

            var textureObjects = bindings
                .Select(binding => (Object)binding.Texture)
                .Where(texture => texture != null)
                .ToArray();
            if (textureObjects.Length > 0)
                Undo.RecordObjects(textureObjects, $"Bake {displayPropName}");

            foreach (var binding in bindings)
            {
                var texture = binding.Texture;
                Bake(binding.Data, ref texture, _textureOptions, textureName);
                binding.Texture = texture;

                if (binding.Data.ApplyStoredTextureSettings(texture))
                    EditorUtility.SetDirty(texture);

                EditorUtility.SetDirty(binding.Data);
                if (texture != null)
                    EditorUtility.SetDirty(texture);
                EditorUtility.SetDirty(binding.Material);
            }
        }

        private static bool HasTextureSettingMismatch(GeneratedTextureBinding<TData>[] bindings)
        {
            return bindings.Any(binding => binding.Data.HasSamplerStateMismatch(binding.Texture));
        }

        private static bool HasColorSpaceMismatch(GeneratedTextureBinding<TData>[] bindings)
        {
            return bindings.Any(binding => binding.Data.HasColorSpaceMismatch(binding.Texture));
        }

        private void DrawTextureSettingsFields(Rect position, SerializedObject so)
        {
            var wrapModeProperty = so.FindProperty("_wrapMode");
            var filterModeProperty = so.FindProperty("_filterMode");
            var anisoLevelProperty = so.FindProperty("_anisoLevel");
            var colorSpaceProperty = so.FindProperty("_colorSpace");

            var rowHeight = GUIHelper.SingleLineHeight;
            var rowSpacing = GUIHelper.VerticalSpacing;
            var columnGap = ContentSpacing;
            var columnWidth = Mathf.Max(0f, (position.width - columnGap) * 0.5f);

            var wrapRect = new Rect(position.x, position.y, columnWidth, rowHeight);
            var filterRect = new Rect(wrapRect.xMax + columnGap, position.y, columnWidth, rowHeight);
            var anisoRect = new Rect(position.x, position.y + rowHeight + rowSpacing, columnWidth, rowHeight);
            var colorSpaceRect = new Rect(anisoRect.xMax + columnGap, anisoRect.y, columnWidth, rowHeight);

            DrawCompactPropertyField(wrapRect, wrapModeProperty, "Wrap");
            DrawCompactPropertyField(filterRect, filterModeProperty, "Filter");
            DrawCompactIntSlider(anisoRect, anisoLevelProperty, 0, 16, "Aniso");
            DrawCompactPropertyField(colorSpaceRect, colorSpaceProperty, "Color Space");
        }

        private static void DrawCompactPropertyField(Rect position, SerializedProperty property, string label)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.45f, 72f);
            EditorGUI.PropertyField(position, property, new GUIContent(label));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private static void DrawCompactIntSlider(Rect position, SerializedProperty property, int leftValue, int rightValue, string label)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.45f, 72f);
            EditorGUI.IntSlider(position, property, leftValue, rightValue, new GUIContent(label));
            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        private static void DrawTexturePreview(Rect previewRect, MaterialProperty prop, MaterialEditor editor)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.showMixedValue = prop.hasMixedValue;
            editor.TextureProperty(previewRect, prop, string.Empty, false);
            EditorGUI.showMixedValue = false;
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawTilingOffset(Rect tilingOffsetRect, MaterialProperty prop, MaterialEditor editor)
        {
            EditorGUI.showMixedValue = prop.hasMixedValue;
            editor.TextureScaleOffsetProperty(tilingOffsetRect, prop, true);
            EditorGUI.showMixedValue = false;
        }

        private static void DrawLabel(Rect position, string displayPropName)
        {
            var labelWidth = position.width * 0.3f;
            var labelRect = new Rect(position.x, position.y, labelWidth, GUIHelper.SingleLineHeight);
            EditorGUI.LabelField(labelRect, displayPropName);
        }

        private static Rect GetContentRect(Rect position)
        {
            var labelWidth = position.width * 0.3f;
            return new Rect(position.x + labelWidth, position.y, position.width - labelWidth + GUIHelper.IndentWidth * 2f, position.height);
        }

        private Rect GetDetailRect(Rect contentRect)
        {
            var previewWidth = GUIHelper.TexturePropertyHeight;
            var minTilingWidth = 96f;
            var maxDetailWidth = Mathf.Max(120f, contentRect.width - previewWidth - ContentSpacing * 2f - minTilingWidth);
            var detailWidth = Mathf.Clamp(contentRect.width * 0.35f, 120f, maxDetailWidth);
            return new Rect(contentRect.x, contentRect.y, detailWidth, GetDetailHeight());
        }

        private static Rect GetPreviewRect(Rect contentRect, Rect detailRect)
        {
            return new Rect(detailRect.xMax + ContentSpacing, contentRect.y, GUIHelper.TexturePropertyHeight, GUIHelper.TexturePropertyHeight);
        }

        private static Rect GetTilingOffsetRect(Rect contentRect, Rect previewRect)
        {
            var tilingOffsetX = previewRect.xMax + ContentSpacing;
            var tilingOffsetY = contentRect.y + (GUIHelper.TexturePropertyHeight - GUIHelper.TillingOffsetPropertyHeight) * 0.5f;
            var tilingOffsetWidth = Mathf.Max(0f, contentRect.xMax - tilingOffsetX);
            return new Rect(tilingOffsetX, tilingOffsetY, tilingOffsetWidth, GUIHelper.TillingOffsetPropertyHeight);
        }

        private static Rect GetSettingsRect(Rect contentRect, float topAreaHeight)
        {
            return new Rect(
                contentRect.x,
                contentRect.y + topAreaHeight + GUIHelper.VerticalSpacing,
                contentRect.width,
                GUIHelper.GetLineBlockHeight(2));
        }

        private float CalculatePropertyHeight()
        {
            return Mathf.Max(GetDetailHeight(), GUIHelper.TexturePropertyHeight) +
                   GUIHelper.VerticalSpacing +
                   GUIHelper.GetLineBlockHeight(2);
        }

        private float GetDetailHeight()
        {
            return GUIHelper.GetLineBlockHeight(DetailLineCount);
        }
    }
}
