using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DotNetConsoleApp;

internal class Linq(ILogger<Linq> logger)
{
	private readonly ILogger _logger = logger;

	public void ShowLinqGroupBy()
	{
		var vehicles = new[]
		{
			new { Make = "Chevrolet", Model = "Corvette", Year = 2000 },
			new { Make = "Dodge", Model = "Neon", Year = 2014 },
			new { Make = "Dodge", Model = "Dart", Year = 2012 },
			new { Make = "Dodge", Model = "Ram", Year = 2012 },
			new { Make = "GMC", Model = "Terrain", Year = 2014 },
			new { Make = "Honda", Model = "Accord", Year = 2014 },
			new { Make = "Honda", Model = "Civic", Year = 2020 }
		};

		var vehiclesByMake = vehicles
			.GroupBy(x => x.Make)
			.Select(g => new { Make = g.Key, NumVehicles = g.Count() })
			.OrderByDescending(g => g.NumVehicles)
			.ThenBy(g => g.Make)
			.ToList();

		var vehiclesByYear = (
			from vehicle in vehicles
			group vehicle by vehicle.Year into g
			let count = g.Count()
			let year = g.Key
			orderby count descending, year
			select new { Year = year, NumVehicles = count }
		).ToList();

		_logger.LogInformation("{vehiclesByMake}", 
			JsonSerializer.Serialize(vehiclesByMake, new JsonSerializerOptions { WriteIndented = true })
		);

		_logger.LogInformation("{vehiclesByYear}",
			JsonSerializer.Serialize(vehiclesByYear, new JsonSerializerOptions { WriteIndented = true })
		);
	}
}
