/*
 * Copyright 2016-2019 Mohawk College of Applied Arts and Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: Nityan Khanna
 * Date: 2019-3-12
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using FileLogger;
using Microsoft.Extensions.Configuration;

namespace Week9WebSockets
{
	/// <summary>
	/// Represents startup tasks for the application.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// The configuration.
		/// </summary>
		private readonly IConfiguration configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="Startup"/> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		/// <summary>
		/// Configures the specified application.
		/// </summary>
		/// <param name="app">The application.</param>
		/// <param name="environment">The environment.</param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment environment)
		{
			if (environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// setup our web socket options
			// indicate to the application that we want to specify a 2 minute keep alive interval
			//	keep alive interval
			//		allows us to specify the time span that we want to keep the connection
			//	receive buffer size
			//		indicates that we want to receive chunks of information at a time
			//		the size of the chunk of information to process upon having a client application
			//		connect, is 4 * 1024 = ~4 KB
			var webSocketOptions = new WebSocketOptions
			{
				KeepAliveInterval = TimeSpan.FromSeconds(180),
				ReceiveBufferSize = 4 * 1024
			};

			// indicate to our application that we want to use
			// web sockets as a part of the processing of data
			app.UseWebSockets(webSocketOptions);
			app.UseMiddleware<WebSocketMiddleware>();

			// indicate to our application that we support the 
			// serving of static files, such as css, html, js, etc.
			app.UseStaticFiles();
		}

		/// <summary>
		/// Configures the services.
		/// </summary>
		/// <param name="services">The services.</param>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging(logging =>
			{
				logging.ClearProviders();
				logging.AddConsole();
				logging.SetMinimumLevel(Enum.Parse<LogLevel>(this.configuration.GetValue<string>("logging:minimumLevel")));

				logging.AddFile(options =>
				{
					options.FileLogSwitches = this.configuration.GetSection("logging:switches:switch").GetChildren().Select(c => new FileLogSwitch(c.Key, Enum.Parse<LogLevel>(c.Value)));
					options.FilePath = this.configuration.GetValue<string>("logging:path");
				});
			});
		}
	}
}