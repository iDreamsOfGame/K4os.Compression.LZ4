using System;
using System.IO;
using System.Text;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract

namespace K4os.Compression.LZ4.Utilities
{
    /// <summary>
    /// Provides methods for compressing and decompressing data by using the LZ4 algorithm.
    /// </summary>
    public static class LZ4Compression
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>
        /// Compress binary data into binary data by using the LZ4 algorithm.
        /// </summary>
        /// <param name="input">The original binary data. </param>
        /// <param name="level">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing data to the stream. </param>
        /// <returns>The compressed binary data. </returns>
        public static byte[] Compress(byte[] input, LZ4Level level = LZ4Level.L00_FAST)
        {
            if (input == null || input.Length == 0)
                return null;

            var target = new byte[LZ4Codec.MaximumOutputSize(input.Length)];
            var encodedLength = LZ4Codec.Encode(
                input,
                0,
                input.Length,
                target,
                0,
                target.Length,
                level);
            using var output = new MemoryStream();
            output.Write(target, 0, encodedLength);
            return output.ToArray();
        }

        /// <summary>
        /// Compress <see cref="string"/> which encoded in <c>encoding</c> into binary data by using the LZ4 algorithm.
        /// </summary>
        /// <param name="source">The original <see cref="string"/>. </param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to get binary data from the original <see cref="string"/>. </param>
        /// <param name="level">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing data to the stream. </param>
        /// <returns>The compressed binary data. </returns>
        public static byte[] Compress(string source, Encoding encoding = null, LZ4Level level = LZ4Level.L00_FAST)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            encoding ??= DefaultEncoding;
            var input = encoding.GetBytes(source);
            return Compress(input, level);
        }

        /// <summary>
        /// Compress binary data into Base64 <see cref="string"/> by using the LZ4 algorithm.
        /// </summary>
        /// <param name="input">The original binary data. </param>
        /// <param name="level">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing data to the stream. </param>
        /// <returns>The compressed data which converted to Base64 <see cref="string"/>. </returns>
        public static string CompressToBase64String(byte[] input, LZ4Level level = LZ4Level.L00_FAST)
        {
            var output = Compress(input, level);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Compress <see cref="string"/> which encoded in <c>encoding</c> into Base64 <see cref="string"/> by using the LZ4 algorithm.
        /// </summary>
        /// <param name="source">The original <see cref="string"/>. </param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to get binary data from the original <see cref="string"/>. </param>
        /// <param name="level">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing data to the stream. </param>
        /// <returns>The compressed data which converted to Base64 <see cref="string"/>. </returns>
        public static string CompressToBase64String(string source, Encoding encoding = null, LZ4Level level = LZ4Level.L00_FAST)
        {
            var output = Compress(source, encoding, level);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// Decompress into original binary data from input binary data by using the LZ4 algorithm.
        /// </summary>
        /// <param name="input">The compressed binary data. </param>
        /// <returns>The original data. </returns>
        public static byte[] Decompress(byte[] input)
        {
            var target = new byte[input.Length * 255];
            var decoded = LZ4Codec.Decode(
                input,
                0,
                input.Length,
                target,
                0,
                target.Length);
            using var output = new MemoryStream();
            output.Write(target, 0, decoded);
            return output.ToArray();
        }

        /// <summary>
        /// Decompress into <see cref="string"/> which encoded in <c>encoding</c> from input binary data by using the LZ4 algorithm.
        /// </summary>
        /// <param name="input">The compressed binary data. </param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to get binary data from the original binary data. </param>
        /// <returns>The original <see cref="string"/>. </returns>
        public static string Decompress(byte[] input, Encoding encoding)
        {
            encoding ??= DefaultEncoding;
            var output = Decompress(input);
            return encoding.GetString(output);
        }

        /// <summary>
        /// Decompress into binary data from Base64 <see cref="string"/> by using the LZ4 algorithm.
        /// </summary>
        /// <param name="source">The Bass64 <see cref="string"/>. </param>
        /// <returns>The original data. </returns>
        public static byte[] DecompressFromBase64String(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            var input = Convert.FromBase64String(source);
            return Decompress(input);
        }

        /// <summary>
        /// Decompress into <see cref="string"/> which encoded in <c>encoding</c> from Base64 <see cref="string"/> by using the LZ4 algorithm.
        /// </summary>
        /// <param name="source">The Bass64 <see cref="string"/>. </param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to get binary data from the original binary data. </param>
        /// <returns>The original <see cref="string"/>. </returns>
        public static string DecompressFromBase64String(string source, Encoding encoding)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            var input = Convert.FromBase64String(source);
            return Decompress(input, encoding);
        }
    }
}