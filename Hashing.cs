using Bogus;
using Microsoft.Extensions.Logging;
using System.IO.Hashing;
using System.Security.Cryptography;

namespace DotNetConsoleApp;

internal class Hashing(ILogger<Hashing> logger)
{
	private readonly ILogger<Hashing> _logger = logger;

	public void ShowHashing()
	{
		byte[] input = new Faker().Random.Bytes(500);
		byte[] crc = Crc32.Hash(input);
		byte[] sha = SHA256.HashData(input);
		byte[] md5 = MD5.HashData(input);

		_logger.LogInformation("CRC32: {crc32}", BitConverter.ToString(crc).Replace("-", ""));
		_logger.LogInformation("SHA-256: {sha}", BitConverter.ToString(sha).Replace("-", ""));
		_logger.LogInformation("MD5: {md5}", BitConverter.ToString(md5).Replace("-", ""));
	}
}
