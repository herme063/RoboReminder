using System;

namespace RoboReminderWeb
{
	internal class HangfireActivator : Hangfire.JobActivator
	{
		private readonly IServiceProvider _serviceProvider;

		public HangfireActivator(IServiceProvider serviceProvider)
		{
			this._serviceProvider = serviceProvider;
		}

		public override object ActivateJob(Type jobType)
		{
			return _serviceProvider.GetService(jobType);
		}
	}
}