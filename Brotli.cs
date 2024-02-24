using System.Buffers;
using System.Globalization;
using System.IO.Compression;
using System.Text;

namespace DotNetConsoleApp;

internal static class Brotli
{
	const int BUFFER_SIZE = 0x4000;

	public static async Task CompressAsync(Stream inputStream, Stream outputStream)
	{
		Memory<byte> buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
		using BrotliStream compressionStream = new(outputStream, CompressionMode.Compress);
		int count;
		while ((count = await inputStream.ReadAsync(buffer)) > 0)
		{
			await compressionStream.WriteAsync(buffer[..count]);
		}
	}

	public static async Task DecompressAsync(Stream inputStream, Stream outputStream)
	{
		Memory<byte> buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
		using BrotliStream compressionStream = new(inputStream, CompressionMode.Decompress);
		int count;
		while ((count = await compressionStream.ReadAsync(buffer)) > 0)
		{
			await outputStream.WriteAsync(buffer[..count]);
		}
	}

	public static async Task<ReadOnlyMemory<byte>> CompressAsync(ReadOnlyMemory<byte> input)
	{
		using MemoryStream inputStream = new(input.Length);
		inputStream.Write(input.Span);
		inputStream.Position = 0;
		using MemoryStream outputStream = new();
		await CompressAsync(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static async Task<ReadOnlyMemory<byte>> DecompressAsync(ReadOnlyMemory<byte> input)
	{
		using MemoryStream inputStream = new(input.Length);
		inputStream.Write(input.Span);
		inputStream.Position = 0;
		using MemoryStream outputStream = new();
		await DecompressAsync(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static async Task<byte[]> CompressAsync(byte[] input)
	{
		using MemoryStream inputStream = new(input);
		using MemoryStream outputStream = new();
		await CompressAsync(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static async Task<byte[]> DecompressAsync(byte[] input)
	{
		using MemoryStream inputStream = new(input);
		using MemoryStream outputStream = new();
		await DecompressAsync(inputStream, outputStream);
		return outputStream.ToArray();
	}

	public static async Task<ReadOnlyMemory<byte>> CompressStringAsync(string input)
		=> await CompressAsync(Encoding.UTF8.GetBytes(input));
	public static async Task<string> DecompressToStringAsync(ReadOnlyMemory<byte> input)
		=> Encoding.UTF8.GetString((await DecompressAsync(input)).Span);

	public static async Task CompressFileAsync(string inputFileName)
	{
		string outputFileName = $"{inputFileName}.br";
		if (File.Exists(outputFileName))
		{
			throw new IOException($"File {outputFileName} already exists");
		}
		using FileStream inputStream = File.OpenRead(inputFileName);
		using FileStream outputStream = File.OpenWrite(outputFileName);
		await CompressAsync(inputStream, outputStream);
	}

	public static async Task DecompressFileAsync(string inputFileName)
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
		await DecompressAsync(inputStream, outputStream);
	}
}
