using Bogus;
using DotNetConsoleApp;
using DotNetConsoleApp.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;
using System.Text.Json;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

// VS: Manage User Secrets
// CLI: dotnet user-secrets init; dotnet user-secrets set "AppSettings:Number" 888
builder.Configuration.AddUserSecrets<Program>();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(builder.Configuration)
	.CreateLogger();

builder.Services.AddLogging(builder =>
{
	builder.ClearProviders();
	builder.AddSerilog();
});

builder.Services.AddAppSettings(builder.Configuration);
builder.Services.AddThirdPartyService(options =>
{
	options.ProviderType = "SOME OTHER PROVIDER";
});

builder.Services.AddScoped<MyDisposable>();

builder.Services.AddDbContext<MyDatabaseContext>(options =>
{
	options.UseSqlite(builder.Configuration.GetConnectionString("MyDatabase")!);
});

builder.Services.AddAcmeSettings(builder.Configuration);

builder.Services.AddHostedService<Program>();

await builder.Build().RunAsync();

partial class Program(
	MyDatabaseContext dbContext,
	IOptions<AppSettings> appSettings,
	ILogger<Program> logger,
	ThirdPartyService thirdPartyService,
	IServiceProvider serviceProvider,
	AcmeService acmeService
) : BackgroundService
{
	private readonly MyDatabaseContext _dbContext = dbContext;
	private readonly IOptions<AppSettings> _appSettings = appSettings;
	private readonly ILogger<Program> _logger = logger;
	private readonly ThirdPartyService _thirdPartyService = thirdPartyService;
	private readonly IServiceProvider _serviceProvider = serviceProvider;
	private readonly AcmeService _acmeService = acmeService;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			ShowConfig();
			ShowThirdPartyService();
			ShowDisposable();
			await ShowEntityFrameworkAsync(stoppingToken);
			await ShowAcmeAsync(stoppingToken);
			ShowBogus();
			ShowBrotli();
			ShowGZip();
			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "An error occurred");
			Environment.Exit(1);
		}
	}

	private void ShowConfig()
	{
		_logger.LogDebug("{Name} | {Number} | {IsSomething}",
			_appSettings.Value.Name, _appSettings.Value.Number, _appSettings.Value.IsSomething);

		foreach (Thing thing in _appSettings.Value.Things)
		{
			_logger.LogDebug("{Thing}", thing);
		}
	}

	private void ShowThirdPartyService()
	{
		_logger.LogInformation("ProviderType: {ProviderType}", _thirdPartyService.ProviderType);
		_logger.LogInformation("TimeoutSeconds: {TimeoutSeconds}", _thirdPartyService.TimeoutSeconds);
	}

	private void ShowDisposable()
	{
		using IServiceScope scope = _serviceProvider.CreateScope();

		// This gets disposed when the scope is disposed
		MyDisposable myDisposable = scope.ServiceProvider.GetRequiredService<MyDisposable>();
	}

	private async Task ShowEntityFrameworkAsync(CancellationToken stoppingToken)
	{
		await _dbContext.Database.EnsureCreatedAsync(stoppingToken);
		Foo foo1 = new() { Name = "First Foo" };
		foo1.Bars.Add(new() { Name = "First Bar" });
		foo1.Bars.Add(new() { Name = "Second Bar" });
		_dbContext.Foos.Add(foo1);
		foo1 = new() { Name = "Second Foo" };
		_dbContext.Foos.Add(foo1);
		await _dbContext.SaveChangesAsync(stoppingToken);

		foreach (Foo foo in await _dbContext.Foos.Include(foo => foo.Bars).ToListAsync(cancellationToken: stoppingToken))
		{
			_logger.LogInformation("Foo: {Foo}", foo.Name);
			foreach (Bar bar in foo.Bars)
			{
				_logger.LogInformation("Bar: {Bar}", bar.Name);
			}
		}
	}

	private async Task ShowAcmeAsync(CancellationToken stoppingToken)
	{
		for (int i = 0; i < 3; i++)
		{
			await _acmeService.DoSomethingAsync(stoppingToken);
		}
	}

	private void ShowBogus()
	{
		BogusPerson person = BogusPerson.Generate();
		_logger.LogInformation("{person}", JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true }));
	}

	private void ShowBrotli()
	{
		byte[] inputBytes = Encoding.UTF8.GetBytes(
			string.Join(" ", Enumerable.Range(0, 500).Select(i => new Faker().Lorem.Word()))
		);
		byte[] compressedBytes = Brotli.Compress(inputBytes);
		byte[] outputBytes = Brotli.Decompress(compressedBytes);
		_logger.LogInformation("Input length: {inputLength}, Compressed length: {compressedLength}",
			inputBytes.Length.ToString("#,##0"), compressedBytes.Length.ToString("#,##0"));
		_logger.LogInformation("Input matches output: {isMatch}", inputBytes.SequenceEqual(outputBytes));
	}

	private void ShowGZip()
	{
		byte[] inputBytes = Encoding.UTF8.GetBytes(
			string.Join(" ", Enumerable.Range(0, 500).Select(i => new Faker().Lorem.Word()))
		);
		byte[] compressedBytes = GZip.Compress(inputBytes);
		byte[] outputBytes = GZip.Decompress(compressedBytes);
		_logger.LogInformation("Input length: {inputLength}, Compressed length: {compressedLength}",
			inputBytes.Length.ToString("#,##0"), compressedBytes.Length.ToString("#,##0"));
		_logger.LogInformation("Input matches output: {isMatch}", inputBytes.SequenceEqual(outputBytes));
	}
}
