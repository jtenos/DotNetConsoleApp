using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetConsoleApp;

internal class AcmeService(HttpClient httpClient, ILogger<AcmeService> logger)
{
	private readonly HttpClient _httpClient = httpClient;
	private readonly ILogger _logger = logger;

	public async Task DoSomethingAsync(CancellationToken stoppingToken)
	{
		HttpRequestMessage requestMessage = new(HttpMethod.Get, "/todos/1");
		HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage, stoppingToken);
		_logger.LogInformation("Token: {token}", requestMessage.Headers.Authorization);
		responseMessage.EnsureSuccessStatusCode();
		string json = await responseMessage.Content.ReadAsStringAsync(stoppingToken);
		Todo todo = JsonSerializer.Deserialize<Todo>(json)!;
		_logger.LogInformation("Todo: {todo}", todo);
	}
}

internal static class AcmeServiceExtensions
{
	public static IServiceCollection AddAcmeSettings(this IServiceCollection services, IConfiguration config)
	{
		services.Configure<AcmeSettings>(config.GetSection(nameof(AcmeSettings)));

		services.AddTransient<TokenHandler>();

		// This also registers AcmeService as a transient service
		services.AddHttpClient<AcmeService>((serviceProvider, client) =>
		{
			AcmeSettings acmeSettings = serviceProvider.GetRequiredService<IOptions<AcmeSettings>>().Value;
			client.BaseAddress = new(acmeSettings.BaseUrl);
			client.DefaultRequestHeaders.Add("User-Agent", acmeSettings.UserAgent);
		}).AddHttpMessageHandler<TokenHandler>();

		services.AddSingleton<AcmeTokenService>();

		return services;
	}
}

internal class AcmeSettings
{
	public string BaseUrl { get; set; } = string.Empty;
	public string UserAgent { get; set; } = string.Empty;
}

internal record class Todo
{
	[JsonPropertyName("userId")]
	public int UserId { get; set; }

	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; } = "";

	[JsonPropertyName("completed")]
	public bool Completed { get; set; }
}

/// <summary>
/// This will run before each request is set, so you can set an authentication token here.
/// </summary>
/// <param name="acmeTokenService"></param>
public class TokenHandler(AcmeTokenService acmeTokenService)
	: DelegatingHandler
{
	private readonly AcmeTokenService _acmeTokenService = acmeTokenService;

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		string newToken = _acmeTokenService.GetNewToken();
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

		return await base.SendAsync(request, cancellationToken);
	}
}

public class AcmeTokenService
{
	public string GetNewToken() => $"{DateTime.Now:HHmmssfff}";
}
