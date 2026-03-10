using UnityEngine;

namespace ExtEditor.UberMaterialPropertyDrawer
{
    internal sealed class GeneratedTextureNormalizationResult
    {
        public GeneratedTextureNormalizationResult(
            bool isValid,
            string warning,
            Texture normalizedTextureSource,
            bool metadataChanged,
            bool settingsChanged,
            bool visibilityChanged,
            bool requiresTextureRecreation)
        {
            IsValid = isValid;
            Warning = warning;
            NormalizedTextureSource = normalizedTextureSource;
            MetadataChanged = metadataChanged;
            SettingsChanged = settingsChanged;
            VisibilityChanged = visibilityChanged;
            RequiresTextureRecreation = requiresTextureRecreation;
        }

        public bool IsValid { get; }
        public string Warning { get; }
        public Texture NormalizedTextureSource { get; }
        public bool MetadataChanged { get; }
        public bool SettingsChanged { get; }
        public bool VisibilityChanged { get; }
        public bool RequiresTextureRecreation { get; }

        public bool HasAnyChange => MetadataChanged || SettingsChanged || VisibilityChanged;
    }
}