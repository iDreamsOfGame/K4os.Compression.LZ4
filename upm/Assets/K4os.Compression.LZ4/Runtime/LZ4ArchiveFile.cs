using System.IO;
using System.Text;
using UnityEngine;

namespace K4os.Compression.LZ4.Utilities
{
    /// <summary>
    /// Provides methods for compressing and decompressing files under folder into archive file by using the LZ4 algorithm.
    /// </summary>
    public static class LZ4ArchiveFile
    {
        /// <summary>
        /// Recursively packs an entire folder, preserves directory structure, outputs a single lz4 archive file.
        /// </summary>
        /// <param name="sourceFolderPath">Source folder path. </param>
        /// <param name="archiveOutputPath">Output path of the archive file. </param>
        /// <param name="level">LZ4 compression level. </param>
        /// <param name="searchPattern">File filter, *.* for all files. </param>
        public static void PackFolder(string sourceFolderPath,
            string archiveOutputPath,
            LZ4Level level = LZ4Level.L00_FAST,
            string searchPattern = "*.*")
        {
            sourceFolderPath = Path.GetFullPath(sourceFolderPath);
            if (!Directory.Exists(sourceFolderPath))
                throw new DirectoryNotFoundException(sourceFolderPath);

            var allFiles = Directory.GetFiles(sourceFolderPath, searchPattern, SearchOption.AllDirectories);
            using var fileStream = File.Create(archiveOutputPath);
            using var binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8, leaveOpen: true);
            binaryWriter.Write(allFiles.Length);

            foreach (var absoluteFilePath in allFiles)
            {
                var relativePath = Path.GetRelativePath(sourceFolderPath, absoluteFilePath);
                var relativePathBytes = Encoding.UTF8.GetBytes(relativePath);
                var rawSize = new FileInfo(absoluteFilePath).Length;

                binaryWriter.Write(relativePathBytes.Length);
                binaryWriter.Write(relativePathBytes);
                binaryWriter.Write(rawSize);

                var compressedData = LZ4Compression.CompressFile(absoluteFilePath, level);
                binaryWriter.Write(compressedData.Length);
                binaryWriter.Write(compressedData);
            }
        }

        /// <summary>
        /// Unpacks archive file and automatically reconstructs directory structure.
        /// </summary>
        /// <param name="archiveFilePath">Path to the archive file. </param>
        /// <param name="extractRootDir">Root directory for extraction. </param>
        public static void UnpackFolder(string archiveFilePath, string extractRootDir)
        {
            if (!Directory.Exists(extractRootDir))
                Directory.CreateDirectory(extractRootDir);

            using var archiveFileStream = File.OpenRead(archiveFilePath);
            UnpackFolder(archiveFileStream, extractRootDir);
        }

        /// <summary>
        /// Unpacks archive file raw data and automatically reconstructs directory structure.
        /// </summary>
        /// <param name="archiveFileRawData">Archive file raw data. </param>
        /// <param name="extractRootDir">Root directory for extraction. </param>
        public static void UnpackFolder(byte[] archiveFileRawData, string extractRootDir)
        {
            var archiveFileStream = new MemoryStream(archiveFileRawData);
            UnpackFolder(archiveFileStream, extractRootDir);
        }

        /// <summary>
        /// Unpacks archive file stream and automatically reconstructs directory structure.
        /// </summary>
        /// <param name="archiveFileStream">Archive file stream. </param>
        /// <param name="extractRootDir">Root directory for extraction. </param>
        public static void UnpackFolder(Stream archiveFileStream, string extractRootDir)
        {
            using var binaryReader = new BinaryReader(archiveFileStream, Encoding.UTF8, leaveOpen: true);
            var totalFiles = binaryReader.ReadInt32();
            for (var i = 0; i < totalFiles; i++)
            {
                var pathBytesLength = binaryReader.ReadInt32();
                var pathBytes = binaryReader.ReadBytes(pathBytesLength);
                var relativePath = Encoding.UTF8.GetString(pathBytes);
                var rawSize = binaryReader.ReadInt64();
                var compressedSize = binaryReader.ReadInt32();
                var outFullPath = Path.Combine(extractRootDir, relativePath);
                var outputDir = Path.GetDirectoryName(outFullPath);

                if (string.IsNullOrEmpty(outputDir))
                {
                    Debug.LogError("Output directory is null or empty, can not create.");
                    continue;
                }

                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                var compressedData = binaryReader.ReadBytes(compressedSize);
                var decompressedData = LZ4Compression.Decompress(compressedData);
                if (decompressedData.Length != rawSize)
                {
                    Debug.LogError($"Decompressed size mismatch. output file: {outFullPath}");
                    continue;
                }

                File.WriteAllBytes(outFullPath, decompressedData);
            }
            
            archiveFileStream.Dispose();
        }
    }
}