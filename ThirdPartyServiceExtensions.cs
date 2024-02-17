using Microsoft.Extensions.DependencyInjection;

namespace DotNetConsoleApp;

internal static class ThirdPartyServiceExtensions
{
	public static IServiceCollection AddThirdPartyService(
		this IServiceCollection services, Action<ThirdPartyService>? options = null)
	{
		if (options is null) return services.AddSingleton<ThirdPartyService>();
		
		ThirdPartyService thirdPartyService = new();
		options(thirdPartyService);
		return services.AddSingleton(thirdPartyService);
	}
}
