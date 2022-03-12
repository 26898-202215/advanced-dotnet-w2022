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
 * Date: 2019-3-7
 */

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Week9WebSockets
{
	/// <summary>
	/// Represents the main program.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static async Task Main(string[] args)
		{
			var builder = CreateWebHostBuilder(args).Build();

			var entryAssembly = Assembly.GetEntryAssembly();

			var logger = builder.Services.GetService<ILogger<Program>>();

			logger.LogInformation($"Week 9 Demo: v{entryAssembly.GetName().Version}");
			logger.LogInformation($"Week 9 Demo Working Directory : {Path.GetDirectoryName(entryAssembly.Location)}");
			logger.LogInformation($"Operating System: {Environment.OSVersion.Platform} {Environment.OSVersion.VersionString}");
			logger.LogInformation($"CLI Version: {Environment.Version}");
			logger.LogInformation($"{entryAssembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}");

			await builder.RunAsync();
		}

		/// <summary>
		/// Creates the web host builder.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns>Returns the created web host builder instance.</returns>
		private static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			var workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			Directory.SetCurrentDirectory(workingDirectory);

			var builder = WebHost.CreateDefaultBuilder(args)
								.UseStartup<Startup>()
								.ConfigureAppConfiguration((context, config) =>
								{
									config.SetBasePath(workingDirectory);
									config.AddXmlFile(Path.Combine(workingDirectory, "app.config"), false, true);
								});

			return builder;
		}
	}
}