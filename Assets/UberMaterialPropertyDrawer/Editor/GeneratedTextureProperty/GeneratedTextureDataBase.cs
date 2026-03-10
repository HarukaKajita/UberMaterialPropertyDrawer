using System;
using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    public abstract class GeneratedTextureDataBase : ScriptableObject
    {
        public const string CurveTextureGeneratorKind = "CurveTexture";
        public const string GradientTextureGeneratorKind = "GradientTexture";

        [SerializeField] private string _sourcePropertyName;
        [SerializeField] private string _generatorKind;
        [SerializeField] private string _generatedTextureName;

        public string SourcePropertyName => _sourcePropertyName;
        public string GeneratorKind => _generatorKind;
        public string GeneratedTextureName => _generatedTextureName;

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

        private static bool SetIfDifferent(ref string currentValue, string nextValue)
        {
            if (string.Equals(currentValue, nextValue, StringComparison.Ordinal)) return false;
            currentValue = nextValue;
            return true;
        }
    }
}
