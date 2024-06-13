using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Hosting;
using MasterServer.ClusterUniqueService;
using Orleans.Configuration;
using StarSolitaireTournament.Grains;

namespace StarSolitaireTournament
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				   .MinimumLevel.Debug()
				   .Enrich.FromLogContext()
				   .WriteTo.Console(theme: AnsiConsoleTheme.Code)
				   .CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
				  Host.CreateDefaultBuilder(args)
				  .UseOrleans(builder =>
				  {
					  //builder.UseLocalhostClustering();
					  builder.AddMemoryGrainStorage(name: "localStorage");
					  builder.UseInMemoryReminderService();

					  string azureTableStorage = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE");
					  //builder.AddAzureBlobGrainStorage(
						 // name: "blobStorage",
						 // configureOptions: options =>
						 // {
							//  options.UseJson = true;
							//  options.ConfigureBlobServiceClient(azureTableStorage);
						 // });

					  //builder.AddAzureTableGrainStorage(
					  //    name: "localStorage",
					  //    configureOptions: options =>
					  //    {
					  //        options.UseJson = true;
					  //        options.ConfigureTableServiceClient(azureTableStorage);
					  //        options.TableName = Environment.GetEnvironmentVariable("MASTER_SERVER_GRAIN_TABLE_NAME");
					  //        //options.DeleteStateOnClear = true;
					  //    });

					  if (bool.Parse(Environment.GetEnvironmentVariable("IS_TO_TEST_GAME_SERVER")))
					  {
						  builder.UseLocalhostClustering();
					  }
					  else
					  {
						  //builder.UseAzureStorageClustering(options =>
						  //{
						  //    options.ConfigureTableServiceClient(azureTableStorage);
						  //    options.TableName = Environment.GetEnvironmentVariable("MASTER_SERVER_CLUSTERING_TABLE_NAME");
						  //});
						  builder.UseLocalhostClustering();
						  builder.UseKubernetesHosting();
					  }
					  ////

					  //builder.UseAzureTableReminderService(
					  //options =>
					  //{
					  //    options.ConfigureTableServiceClient(azureTableStorage);
					  //    options.TableName = Environment.GetEnvironmentVariable("MASTER_SERVER_REMINDER_TABLE_NAME");
					  //});

					  builder.Configure<SchedulingOptions>(options => options.AllowCallChainReentrancy = false);

					  //builder.UseDashboard(options => { });
					  builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(LeaderboardGrain).Assembly).WithReferences());
					  builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(UniqueServiceGrain).Assembly).WithReferences());

					  //builder.ConfigureLogging(logging => logging.AddConsole());
				  })
				 .ConfigureLogging(builder => builder.AddConsole())
				 .ConfigureWebHostDefaults(webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>();
				 }).UseDefaultServiceProvider((context, options) =>
				 {
					 options.ValidateScopes = true;
				 });
	}
}