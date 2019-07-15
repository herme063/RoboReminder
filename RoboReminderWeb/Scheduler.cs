using System;
using System.Linq;
using Hangfire;
using Humanizer;
using TimeZoneConverter;
using Twilio.Clients;

namespace RoboReminderWeb
{
	public class Scheduler
	{
		const string DailyTipJobId = "{0} - Daily Tips";
		const string DailyMantraJobId = "{0} - Daily Mantra";

		private readonly ApplicationSettings _applicationSettings;

		public Scheduler(ApplicationSettings applicationSettings)
		{
			_applicationSettings = applicationSettings;
		}

		public void Schedule()
		{
			foreach (ApplicationSettings.DataConfiguration data in _applicationSettings.Data)
			{
				RecurringJob.RemoveIfExists(DailyTipJobId.FormatWith(data.Username));
				RecurringJob.RemoveIfExists(DailyMantraJobId.FormatWith(data.Username));

				if (data.DailyQuotes.Enabled)
				{
					RecurringJob.AddOrUpdate<Scheduler>(
						DailyTipJobId, 
						s => s.OnDailyTipSchedule(data.Username), 
						data.DailyQuotes.Schedule, 
						TZConvert.GetTimeZoneInfo(data.Timezone));
				}

				if (data.DailyMantra.Enabled)
				{
					RecurringJob.AddOrUpdate<Scheduler>(
						DailyMantraJobId, 
						s => s.OnDailyMantraSchedule(data.Username),
						data.DailyMantra.Schedule,
						TZConvert.GetTimeZoneInfo(data.Timezone));
				}
			}
		}

		public void OnDailyTipSchedule(
			string username)
		{
			ApplicationSettings.DataConfiguration data = _applicationSettings.Data
				.Single(d => d.Username == username);
			string tip = data.DailyQuotes.Quotes
				.OrderBy(t => Guid.NewGuid())
				.First();

			var client = new TwilioRestClient(
				_applicationSettings.Twilio.AccountSID, 
				_applicationSettings.Twilio.AuthToken);

			Twilio.Rest.Api.V2010.Account.MessageResource.Create(
				to: new Twilio.Types.PhoneNumber(data.PhoneNumber),
				from: new Twilio.Types.PhoneNumber(_applicationSettings.Twilio.PhoneNumber),
				body: tip,
				client: client);
		}

		public void OnDailyMantraSchedule(
			string username)
		{
			ApplicationSettings.DataConfiguration data = _applicationSettings.Data
				.Single(d => d.Username == username);
			int mantraIndex = data.DailyMantra.Mantras
				.Select((m, idx) => idx)
				.OrderBy(idx => Guid.NewGuid())
				.First();

			var client = new TwilioRestClient(
				_applicationSettings.Twilio.AccountSID, 
				_applicationSettings.Twilio.AuthToken);

			Twilio.Rest.Api.V2010.Account.CallResource.Create(
				to: new Twilio.Types.PhoneNumber(data.PhoneNumber),
				from: new Twilio.Types.PhoneNumber(_applicationSettings.Twilio.PhoneNumber),
				url: new Uri(_applicationSettings.Twilio.VoiceEndpoint.FormatWith(data.Username, mantraIndex)),
				client: client);
		}
	}
}
