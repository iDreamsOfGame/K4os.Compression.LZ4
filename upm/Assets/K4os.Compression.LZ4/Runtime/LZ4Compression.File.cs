using System;
using System.IO;

namespace K4os.Compression.LZ4.Utilities
{
    /// <summary>
    /// Provides methods for compressing and decompressing file by using the LZ4 algorithm.
    /// </summary>
    public static partial class LZ4Compression
    {
        /// <summary>
        /// Compress file by using the LZ4 algorithm.
        /// </summary>
        /// <param name="filePath">The original file path. </param>
        /// <param name="level">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing data to the stream. </param>
        /// <returns>The compressed binary data. </returns>
        public static byte[] CompressFile(string filePath, LZ4Level level = LZ4Level.L00_FAST)
        {
            if (!File.Exists(filePath))
                return Array.Empty<byte>();
            
            var input = File.ReadAllBytes(filePath);
            return Compress(input, level);
        }
        
        /// <summary>
        /// Decompress file by using the LZ4 algorithm.
        /// </summary>
        /// <param name="filePath">The compressed file path. </param>
        /// <returns>The original binary data. </returns>
        public static byte[] DecompressFile(string filePath)
        {
            if (!File.Exists(filePath))
                return Array.Empty<byte>();
            
            var input = File.ReadAllBytes(filePath);
            return Decompress(input);
        }
    }
}