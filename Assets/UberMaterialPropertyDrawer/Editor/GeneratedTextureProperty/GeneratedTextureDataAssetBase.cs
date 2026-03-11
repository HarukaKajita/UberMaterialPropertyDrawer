using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public abstract class GeneratedTextureDataAssetBase : ScriptableObject
    {
        public const string CurveTextureGeneratorKind = "CurveTexture";
        public const string GradientTextureGeneratorKind = "GradientTexture";

        private const TextureWrapMode DefaultWrapMode = TextureWrapMode.Clamp;
        private const FilterMode DefaultFilterMode = FilterMode.Bilinear;
        private const int DefaultAnisoLevel = 1;
        private const GeneratedTextureColorSpace DefaultColorSpace = GeneratedTextureColorSpace.Linear;

        [SerializeField] private string _sourcePropertyName;
        [SerializeField] private string _generatorKind;
        [SerializeField] private string _generatedTextureName;
        [SerializeField] private TextureWrapMode _wrapMode = DefaultWrapMode;
        [SerializeField] private FilterMode _filterMode = DefaultFilterMode;
        [SerializeField] private int _anisoLevel = DefaultAnisoLevel;
        [SerializeField] private GeneratedTextureColorSpace _colorSpace = DefaultColorSpace;
        [SerializeField] private bool _hasTextureSettings;

        public string SourcePropertyName => _sourcePropertyName;
        public string GeneratorKind => _generatorKind;
        public string GeneratedTextureName => _generatedTextureName;
        internal TextureWrapMode WrapMode => _wrapMode;
        internal FilterMode FilterMode => _filterMode;
        internal int AnisoLevel => Mathf.Clamp(_anisoLevel, 0, 16);
        internal GeneratedTextureColorSpace ColorSpace => _colorSpace;
        internal bool UsesLinearColorSpace => _colorSpace == GeneratedTextureColorSpace.Linear;
        internal bool HasTextureSettings => _hasTextureSettings;

        public bool HasCompleteMetadata =>
            !string.IsNullOrEmpty(_sourcePropertyName) &&
            !string.IsNullOrEmpty(_generatorKind) &&
            !string.IsNullOrEmpty(_generatedTextureName);

        protected abstract string DefaultGeneratorKind { get; }
        protected abstract string DataNameSuffix { get; }
        protected abstract string TextureNameSuffix { get; }

        public bool SyncMetadata(string sourcePropertyName, string generatedTextureName)
        {
            var changed = false;
            changed |= SetIfDifferent(ref _sourcePropertyName, sourcePropertyName);
            changed |= SetIfDifferent(ref _generatorKind, DefaultGeneratorKind);
            changed |= SetIfDifferent(ref _generatedTextureName, generatedTextureName);
            return changed;
        }

        internal bool SyncStoredTextureSettings(
            TextureWrapMode wrapMode,
            FilterMode filterMode,
            int anisoLevel,
            GeneratedTextureColorSpace colorSpace)
        {
            anisoLevel = Mathf.Clamp(anisoLevel, 0, 16);

            var changed = false;
            changed |= SetIfDifferent(ref _wrapMode, wrapMode);
            changed |= SetIfDifferent(ref _filterMode, filterMode);
            changed |= SetIfDifferent(ref _anisoLevel, anisoLevel);
            changed |= SetIfDifferent(ref _colorSpace, colorSpace);
            changed |= SetIfDifferent(ref _hasTextureSettings, true);
            return changed;
        }

        public bool TryBackfillMetadata(out string warning)
        {
            warning = null;
            if (HasCompleteMetadata) return true;

            if (!TryInferSourcePropertyName(name, out var sourcePropertyName))
            {
                warning = $"Unable to infer generated texture metadata from asset name '{name}'.";
                return false;
            }

            SyncMetadata(sourcePropertyName, sourcePropertyName + TextureNameSuffix);
            return true;
        }

        internal bool TryBackfillTextureSettings(Texture sourceTexture)
        {
            if (_hasTextureSettings) return false;

            var wrapMode = sourceTexture != null ? sourceTexture.wrapMode : DefaultWrapMode;
            var filterMode = sourceTexture != null ? sourceTexture.filterMode : DefaultFilterMode;
            var anisoLevel = sourceTexture != null ? Mathf.Clamp(sourceTexture.anisoLevel, 0, 16) : DefaultAnisoLevel;
            var colorSpace = ResolveColorSpace(sourceTexture);
            return SyncStoredTextureSettings(wrapMode, filterMode, anisoLevel, colorSpace);
        }

        internal bool ApplyStoredTextureSettings(Texture texture)
        {
            if (texture == null) return false;

            var changed = false;
            if (texture.wrapMode != WrapMode)
            {
                texture.wrapMode = WrapMode;
                changed = true;
            }

            if (texture.filterMode != FilterMode)
            {
                texture.filterMode = FilterMode;
                changed = true;
            }

            if (texture.anisoLevel != AnisoLevel)
            {
                texture.anisoLevel = AnisoLevel;
                changed = true;
            }

            return changed;
        }

        internal bool HasSamplerStateMismatch(Texture texture)
        {
            if (texture == null) return true;
            return texture.wrapMode != WrapMode || texture.filterMode != FilterMode || texture.anisoLevel != AnisoLevel;
        }

        internal bool HasColorSpaceMismatch(Texture texture)
        {
            if (texture == null) return true;
            var expectsSrgb = ColorSpace == GeneratedTextureColorSpace.Srgb;
            return texture.isDataSRGB != expectsSrgb;
        }

        private bool TryInferSourcePropertyName(string assetName, out string sourcePropertyName)
        {
            sourcePropertyName = null;
            if (string.IsNullOrEmpty(assetName)) return false;
            if (!assetName.EndsWith(DataNameSuffix, StringComparison.Ordinal)) return false;

            var propertyNameLength = assetName.Length - DataNameSuffix.Length;
            if (propertyNameLength <= 0) return false;

            sourcePropertyName = assetName[..propertyNameLength];
            return !string.IsNullOrEmpty(sourcePropertyName);
        }

        private static GeneratedTextureColorSpace ResolveColorSpace(Texture sourceTexture)
        {
            if (sourceTexture == null) return DefaultColorSpace;
            return sourceTexture.isDataSRGB ? GeneratedTextureColorSpace.Srgb : GeneratedTextureColorSpace.Linear;
        }

        private static bool SetIfDifferent<T>(ref T currentValue, T nextValue)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, nextValue)) return false;
            currentValue = nextValue;
            return true;
        }
    }
}

