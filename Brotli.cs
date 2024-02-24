﻿using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace DotNetConsoleApp;

internal static class Brotli
{
	const int BUFFER_SIZE = 0x4000;

	public static void Compress(Stream inputStream, Stream outputStream)
	{
		Span<byte> buffer = stackalloc byte[BUFFER_SIZE];
		using BrotliStream brotli = new(outputStream, CompressionMode.Compress);
		int count;
		while ((count = inputStream.Read(buffer)) > 0)
		{
			brotli.Write(buffer[..count]);
		}
	}

	public static void Decompress(Stream inputStream, Stream outputStream)
	{
		Span<byte> buffer = stackalloc byte[BUFFER_SIZE];
		using BrotliStream brotli = new(inputStream, CompressionMode.Decompress);
		int count;
		while ((count = brotli.Read(buffer)) > 0)
		{
			outputStream.Write(buffer[..count]);
		}
	}

	public static byte[] Compress(byte[] input)
	{
		using MemoryStream inputStream = new(input);
		using MemoryStream outputStream = new();
		Compress(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static byte[] Decompress(byte[] input)
	{
		using MemoryStream inputStream = new(input);
		using MemoryStream outputStream = new();
		Decompress(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static byte[] CompressString(string input) => Compress(Encoding.UTF8.GetBytes(input));
	public static string DecompressToString(byte[] input) => Encoding.UTF8.GetString(Decompress(input));

	public static void CompressFile(string inputFileName)
	{
		string outputFileName = $"{inputFileName}.br";
		if (File.Exists(outputFileName))
		{
			throw new IOException($"File {outputFileName} already exists");
		}
		using FileStream inputStream = File.OpenRead(inputFileName);
		using FileStream outputStream = File.OpenWrite(outputFileName);
		Compress(inputStream, outputStream);
	}

	public static void DecompressFile(string inputFileName)
	{
		if (!inputFileName.EndsWith(".br", ignoreCase: true, CultureInfo.InvariantCulture))
		{
			throw new ArgumentException("File must have .br extension", nameof(inputFileName));
		}
		string outputFileName = inputFileName[..^3];
		if (File.Exists(outputFileName))
		{
			throw new IOException($"File {outputFileName} already exists");
		}
		using FileStream inputStream = File.OpenRead(inputFileName);
		using FileStream outputStream = File.OpenWrite(outputFileName);
		Decompress(inputStream, outputStream);
	}
}