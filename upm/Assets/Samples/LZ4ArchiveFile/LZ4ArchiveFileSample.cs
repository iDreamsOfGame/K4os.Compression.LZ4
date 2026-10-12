using System.IO;
using K4os.Compression.LZ4.Utilities;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace K4os.Compression.LZ4.Samples
{
    public class LZ4ArchiveFileSample : MonoBehaviour
    {
        [SerializeField]
        private Button packButton;

        [SerializeField]
        private Button unpackButton;

        private void Awake()
        {
            packButton.onClick.AddListener(OnPackButtonClicked);
            unpackButton.onClick.AddListener(OnUnpackButtonClicked);
        }

        private void OnDestroy()
        {
            packButton.onClick.RemoveListener(OnPackButtonClicked);
            unpackButton.onClick.RemoveListener(OnUnpackButtonClicked);
        }
        
        private static void OnPackButtonClicked()
        {
            var sourceFolder = Path.Combine(Directory.GetCurrentDirectory(), "SampleAssets", "LZ4ArchiveFile", "Pack");
            var outputFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SampleAssets", "LZ4ArchiveFile", "lz4archive.dat");
            LZ4ArchiveFile.PackFolder(sourceFolder, outputFilePath);
        }
        
        private static void OnUnpackButtonClicked()
        {
            var archiveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "SampleAssets", "LZ4ArchiveFile", "lz4archive.dat");
            var extractRootDir = Path.Combine(Directory.GetCurrentDirectory(), "SampleAssets", "LZ4ArchiveFile", "Unpack");
            LZ4ArchiveFile.UnpackFolder(archiveFilePath, extractRootDir);
        }
    }
}
