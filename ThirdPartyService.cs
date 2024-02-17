namespace DotNetConsoleApp;

/// <summary>
/// Some third party service that you want to use in your application.
/// </summary>
internal class ThirdPartyService
{
	/// <summary>
	/// The provider type. Defaults to "MY PROVIDER".
	/// </summary>
	public string ProviderType { get; set; } = "MY PROVIDER";

	/// <summary>
	/// The timeout in seconds. Defaults to 30.
	/// </summary>
	public int TimeoutSeconds { get; set; } = 30;
}
