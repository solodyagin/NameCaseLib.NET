using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NameCaseLib;
using WebAPI.Models;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DeclensionController : ControllerBase
	{
		// GET api/declension
		[HttpGet]
		public ActionResult<string> Get(string s)
		{
			string res = "";
			if (!string.IsNullOrWhiteSpace(s))
			{
				dynamic obj = new JObject();
				obj.version = Core.Version;
				obj.languageBuild = Ru.LanguageBuild;
				Ru ru = new Ru();
				object[] m = ru.Q(s);
				obj.cases = new JArray(m);
				res = obj.ToString();
			}
			return res;
		}

		// POST api/declension
		[HttpPost]
		public ActionResult<string> Post([FromBody] Payload value)
		{
			dynamic obj = new JObject();
			obj.version = Core.Version;
			obj.languageBuild = Ru.LanguageBuild;
			Ru ru = new Ru();
			object[] m = ru.Q(value.FullName);
			obj.cases = new JArray(m);
			return obj.ToString();
		}
	}
}
