using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RoboReminderWeb
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));
			services.AddScoped<ApplicationSettings>(p => p.GetService<IOptionsSnapshot<ApplicationSettings>>().Value);
			services.AddTransient<Scheduler>();
			//services.AddHangfire(o => o.UseMemoryStorage());
			services.AddHangfire(o => o.UseSqlServerStorage(
				"Server=.\\ENTERPRISE2014;Database=RoboReminder;Integrated Security=True;MultipleActiveResultSets=true;App=webSmartContract"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
		{
			GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));

			app.UseHangfireServer();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Mantra}/{action=Index}");
			});

			// schedule things
			serviceProvider.GetService<Scheduler>().Schedule();
		}
	}
}
