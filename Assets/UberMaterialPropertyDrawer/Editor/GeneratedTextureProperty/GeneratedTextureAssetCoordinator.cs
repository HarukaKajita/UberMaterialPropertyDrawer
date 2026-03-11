using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal sealed class GeneratedTextureAssetCoordinator
    {
        private const HideFlags GeneratedAssetHideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        public GeneratedTextureNormalizationResult Normalize(
            Material material,
            GeneratedTextureDataAssetBase generatedData,
            Texture2D generatedTexture,
            string sourcePropertyName = null,
            string generatedTextureName = null)
        {
            if (generatedData == null)
                return new GeneratedTextureNormalizationResult(false, "Generated texture data is missing.", null, false, false, false, false);

            var metadataChanged = NormalizeMetadata(generatedData, sourcePropertyName, generatedTextureName, out var warning);
            if (warning != null)
                return new GeneratedTextureNormalizationResult(false, warning, generatedTexture, false, false, false, false);

            var normalizedTextureSource = ResolveTextureSource(material, generatedData, generatedTexture);
            var settingsChanged = generatedData.TryBackfillTextureSettings(normalizedTextureSource);
            var visibilityChanged = EnsureGeneratedAssetVisibility(generatedData);
            if (generatedTexture != null)
                visibilityChanged |= EnsureGeneratedAssetVisibility(generatedTexture);

            var requiresTextureRecreation = generatedTexture == null || generatedData.HasColorSpaceMismatch(generatedTexture);
            return new GeneratedTextureNormalizationResult(
                true,
                null,
                normalizedTextureSource,
                metadataChanged,
                settingsChanged,
                visibilityChanged,
                requiresTextureRecreation);
        }

        private static bool NormalizeMetadata(
            GeneratedTextureDataAssetBase generatedData,
            string sourcePropertyName,
            string generatedTextureName,
            out string warning)
        {
            warning = null;

            if (!string.IsNullOrEmpty(sourcePropertyName) && !string.IsNullOrEmpty(generatedTextureName))
                return generatedData.SyncMetadata(sourcePropertyName, generatedTextureName);

            if (generatedData.HasCompleteMetadata)
                return false;

            if (generatedData.TryBackfillMetadata(out warning))
                return true;

            return false;
        }

        private static Texture ResolveTextureSource(Material material, GeneratedTextureDataAssetBase generatedData, Texture2D generatedTexture)
        {
            if (generatedTexture != null)
                return generatedTexture;

            if (material == null)
                return null;

            var sourcePropertyName = generatedData.SourcePropertyName;
            if (string.IsNullOrEmpty(sourcePropertyName) || !material.HasProperty(sourcePropertyName))
                return null;

            return material.GetTexture(sourcePropertyName);
        }

        private static bool EnsureGeneratedAssetVisibility(Object asset)
        {
            if (asset == null)
                return false;

            var nextHideFlags = asset.hideFlags | GeneratedAssetHideFlags;
            if (nextHideFlags == asset.hideFlags)
                return false;

            asset.hideFlags = nextHideFlags;
            return true;
        }
    }
}
