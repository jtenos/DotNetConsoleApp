/*
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DotNetConsoleApp;
internal class DomainAuth
{
	[HttpPost]
	public JsonResult LogIn(string userName, string password)
	{
		UserResult userResult = GetLoginResult(userName, password);
		if (!userResult.Authenticated)
		{
			return Json(new { authenticated = false });
		}

		return Json(new { authenticated = true, userName });
	}

	private UserResult GetLoginResult(string userName, string password)
	{
		const string DOMAIN_NAME = "MYDOMAIN";

		const string DisplayNameAttribute = "DisplayName";
		const string SAMAccountNameAttribute = "SAMAccountName";

		string fullUserName = $@"{DOMAIN_NAME}\{userName}";
		try
		{
			DirectoryEntry entry = new($"LDAP://{DOMAIN_NAME}", fullUserName, password);

			DirectorySearcher searcher = new(entry);

			searcher.Filter = $"({SAMAccountNameAttribute}={userName})";
			searcher.PropertiesToLoad.Add(DisplayNameAttribute);
			searcher.PropertiesToLoad.Add(SAMAccountNameAttribute);
			var result = searcher.FindOne();
			if (result != null)
			{
				var displayName = result.Properties[DisplayNameAttribute];
				var samAccountName = result.Properties[SAMAccountNameAttribute];

				return new() { Authenticated = true, UserName = userName };
			}

			return new() { Authenticated = false };
		}
		catch (Exception ex)
		{
			return new() { Authenticated = false, Error = ex.ToString() };
		}
	}
}
*/