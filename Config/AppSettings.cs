using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace DotNetConsoleApp.Config;

internal record class AppSettings
{
	private IReadOnlyList<Thing> _things = [];

	public required string Name { get; init; }
	public required int Number { get; init; }
	public required bool IsSomething { get; init; }

	public IReadOnlyList<Thing> Things => _things;

	/// <summary>
	/// This binds the __Things property to the configuration property "Things". If you access the __Things
	/// property in code, you'll get a copy of the collection, newly allocated each time, so don't do this. 
	/// But we need to have this property here for the configuration binder to work properly. If you
	/// access the Things property like you should, you'll get the ReadOnlyCollection instance without having to
	/// copy it every time.
	/// </summary>
	[ConfigurationKeyName("Things")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Configuration binding limitations")]
	public required Thing[] __Things
	{
		get => _things.ToArray();
		init => _things = new ReadOnlyCollection<Thing>(value);
	}
}
