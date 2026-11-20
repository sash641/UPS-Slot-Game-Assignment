#if UNITY_EDITOR
using UnityEditor;

namespace JackpotSlots.Editor
{
    /// <summary>Keeps copied assignment art consistently configured as UI-ready sprites.</summary>
    public sealed class SlotArtImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/SlotArt/"))
                return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.maxTextureSize = 2048;
        }
    }
}
#endif
