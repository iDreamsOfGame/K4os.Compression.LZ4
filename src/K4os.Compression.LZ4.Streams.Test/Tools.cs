﻿using System;
using System.IO;
using Xunit;

namespace K4os.Compression.LZ4.Streams.Test
{
	public class Tools
	{
		public static unsafe uint Adler32(byte* data, int length)
		{
			const uint modAdler = 65521;

			uint a = 1, b = 0;

			for (var index = 0; index < length; ++index)
			{
				a = (a + data[index]) % modAdler;
				b = (b + a) % modAdler;
			}

			return (b << 16) | a;
		}

		public static uint Adler32(byte[] data, int index = 0, int length = -1)
		{
			const uint modAdler = 65521;
			if (length < 0)
				length = data.Length - index;

			uint a = 1, b = 0;

			for (; index < length; ++index)
			{
				a = (a + data[index]) % modAdler;
				b = (b + a) % modAdler;
			}

			return (b << 16) | a;
		}

		public static byte[] LoadChunk(string filename, int index, int length)
		{
			using (var file = File.OpenRead(filename))
			{
				length = length < 0 ? (int) (file.Length - index) : length;
				var src = new byte[length];
				file.Seek(index, SeekOrigin.Begin);
				file.Read(src, 0, length);
				return src;
			}
		}

		public static string FindFile(string filename) =>
			Path.Combine(FindRoot(), filename);

		private static string FindRoot(string path = ".")
		{
			bool IsRoot(string p) =>
				Path.GetFullPath(p) == Path.GetFullPath(Path.Combine(p, ".."));

			while (true)
			{
				if (Directory.Exists(Path.Combine(path, "./.git"))) return path;
				if (IsRoot(path)) return null;

				path = Path.Combine(path, "..");
			}
		}

		public static void SameBytes(byte[] source, byte[] target)
		{
			if (source.Length != target.Length)
				throw new ArgumentException($"Arrays are not same length: {source.Length} vs {target.Length}");

			var length = source.Length;
			for (var i = 0; i < length; i++)
			{
				if (source[i] != target[i])
					throw new ArgumentException($"Arrays differ at index {i}: {source[i]} vs {target[i]}");
			}
		}

		public static void SameBytes(byte[] source, byte[] target, int length)
		{
			if (source.Length < length)
				throw new ArgumentException($"Source array is too small: {source.Length}");
			if (target.Length < length)
				throw new ArgumentException($"Target array is too small: {target.Length}");

			for (var i = 0; i < length; i++)
			{
				if (source[i] != target[i])
					throw new ArgumentException($"Arrays differ at index {i}: {source[i]} vs {target[i]}");
			}
		}

		public static void SameFiles(string original, string decoded)
		{
			using (var streamA = File.OpenRead(original))
			using (var streamB = File.OpenRead(decoded))
			{
				Assert.Equal(streamA.Length, streamB.Length);
				var bufferA = new byte[4096];
				var bufferB = new byte[4096];

				while (true)
				{
					var readA = streamA.Read(bufferA, 0, bufferA.Length);
					var readB = streamB.Read(bufferB, 0, bufferB.Length);
					Assert.Equal(readA, readB);
					if (readA == 0)
						break;

					SameBytes(bufferA, bufferB, readA);
				}
			}
		}
	}
}
