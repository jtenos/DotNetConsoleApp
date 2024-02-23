using Bogus;
using Bogus.Extensions.UnitedStates;

namespace DotNetConsoleApp;

internal class BogusPerson
{
	public string FirstName { get; set; } = default!;
	public string LastName { get; set; } = default!;	
	public DateOnly DateOfBirth { get; set; }
	public string Email { get; set; } = default!;
	public string PhoneNumber { get; set; } = default!;
	public string AddressLine1 { get; set; } = default!;
	public string AddressLine2 { get; set; } = default!;
	public string City { get; set; } = default!;
	public string State { get; set; } = default!;
	public string ZipCode { get; set; } = default!;
	public string Country { get; set; } = default!;
	public string AvatarUrl { get; set; } = default!;
	public string SocialSecurityNumber { get; set; } = default!;

	public static BogusPerson Generate()
	{
		return new Faker<BogusPerson>()
			.RuleFor(p => p.FirstName, f => f.Person.FirstName)
			.RuleFor(p => p.LastName, f => f.Person.LastName)
			.RuleFor(p => p.DateOfBirth, f => DateOnly.FromDateTime(f.Person.DateOfBirth))
			.RuleFor(p => p.Email, f => f.Person.Email)
			.RuleFor(p => p.PhoneNumber, f => f.Person.Phone)
			.RuleFor(p => p.AddressLine1, f => f.Address.StreetAddress())
			.RuleFor(p => p.AddressLine2, f => f.Address.SecondaryAddress())
			.RuleFor(p => p.City, f => f.Address.City())
			.RuleFor(p => p.State, f => f.Address.State())
			.RuleFor(p => p.ZipCode, f => f.Address.ZipCode())
			.RuleFor(p => p.Country, f => f.Address.Country())
			.RuleFor(p => p.AvatarUrl, f => f.Person.Avatar)
			.RuleFor(p => p.SocialSecurityNumber, f => f.Person.Ssn())
			.Generate();
	}
}
