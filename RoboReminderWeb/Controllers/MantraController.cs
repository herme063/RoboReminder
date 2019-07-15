using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML;

namespace RoboReminderWeb.Controllers
{
	public class MantraController : Twilio.AspNet.Core.TwilioController
	{
		private readonly ApplicationSettings _applicationSettings;

		public MantraController(ApplicationSettings applicationSettings)
		{
			_applicationSettings = applicationSettings;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Ok("well and alive");
		}

		[HttpPost]
		public IActionResult Recite(
			[FromQuery]string u,
			[FromQuery]int idx)
		{
			ApplicationSettings.DataConfiguration data = _applicationSettings.Data
				.SingleOrDefault(d => d.Username == u);
			if (data != null && 0 <= idx && idx < data.DailyMantra.Mantras.Count)
			{
				string mantra = data.DailyMantra.Mantras[idx];
				var response = new VoiceResponse();
				foreach (string sentence in mantra.Split(new[] { ';', '.' }, StringSplitOptions.RemoveEmptyEntries))
				{
					response.Say(sentence).Pause(2);
				}

				return TwiML(response);
			}
			else
			{
				return NotFound();
			}
		}
	}
}