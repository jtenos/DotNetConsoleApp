using Bogus;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DotNetConsoleApp;

internal class Compression(ILogger<Compression> logger)
{
	private readonly ILogger<Compression> _logger = logger;

	public async Task ShowBrotliAsync()
	{
		ReadOnlyMemory<byte> inputBytes = Encoding.UTF8.GetBytes(
			string.Join(" ", Enumerable.Range(0, 500).Select(i => new Faker().Lorem.Word()))
		);
		ReadOnlyMemory<byte> compressedBytes = await Brotli.CompressAsync(inputBytes);
		ReadOnlyMemory<byte> outputBytes = await Brotli.DecompressAsync(compressedBytes);
		_logger.LogInformation("Input length: {inputLength}, Compressed length: {compressedLength}",
			inputBytes.Length.ToString("#,##0"), compressedBytes.Length.ToString("#,##0"));
		_logger.LogInformation("Input matches output: {isMatch}", inputBytes.Span.SequenceEqual(outputBytes.Span));
	}

	public async Task ShowGZipAsync()
	{
		ReadOnlyMemory<byte> inputBytes = Encoding.UTF8.GetBytes(
			string.Join(" ", Enumerable.Range(0, 500).Select(i => new Faker().Lorem.Word()))
		);
		ReadOnlyMemory<byte> compressedBytes = await GZip.CompressAsync(inputBytes);
		ReadOnlyMemory<byte> outputBytes = await GZip.DecompressAsync(compressedBytes);
		_logger.LogInformation("Input length: {inputLength}, Compressed length: {compressedLength}",
			inputBytes.Length.ToString("#,##0"), compressedBytes.Length.ToString("#,##0"));
		_logger.LogInformation("Input matches output: {isMatch}", inputBytes.Span.SequenceEqual(outputBytes.Span));
	}
}
