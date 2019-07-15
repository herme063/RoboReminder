using System;
using System.Collections.Generic;

namespace RoboReminderWeb
{
	public class ApplicationSettings
	{
		public TwilioConfiguration Twilio { get; set; }
		public List<DataConfiguration> Data { get; set; }

		public class TwilioConfiguration
		{
			public string AccountSID { get; set; }
			public string AuthToken { get; set; }
			public string PhoneNumber { get; set; }
			public string VoiceEndpoint { get; set; }
		}

		public class DataConfiguration
		{
			public string Username { get; set; }
			public string PhoneNumber { get; set; }
			public string Timezone { get; set; }
			public MantraConfiguration DailyMantra { get; set; }
			public QuoteConfiguration DailyQuotes { get; set; }
		}

		public class QuoteConfiguration
		{
			public bool Enabled { get; set; }
			public string Schedule { get; set; }
			public List<string> Quotes { get; set; }
		}

		public class MantraConfiguration
		{
			public bool Enabled { get; set; }
			public string Schedule { get; set; }
			public List<string> Mantras { get; set; }
		}
	}
}
