using MasterServer.ClusterUniqueService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Orleans;
using ServiceStack.Redis;
using StarSolitaireTournament.Authentication;
using StarSolitaireTournament.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StarSolitaireTournament
{
	public class Startup
	{
		private ILogger<Startup> logger;
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public async void ConfigureServices(IServiceCollection services)
		{
			services.AddResponseCompression();

			services.AddLogging();

			services.AddCors(options =>
			{
				options.AddPolicy("AllowOrigin", builder =>
				builder.WithOrigins()
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials()
				.WithHeaders("Authorization")
				.SetIsOriginAllowed((host) => true));
			});

			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
				{
					policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
					policy.RequireClaim(ClaimTypes.NameIdentifier);
				});
			});

			services.AddAuthentication(options =>
			{
				// Identity made Cookie authentication the default.
				// However, we want JWT Bearer Auth to be the default.
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(options =>
				{
					// Configure JWT Bearer Auth to expect our security key
					options.TokenValidationParameters =
					new TokenValidationParameters
					{
						LifetimeValidator = (before, expires, token, param) =>
						{
							return expires > DateTime.UtcNow;
						},
						ValidateAudience = false,
						ValidateIssuer = false,
						ValidateActor = false,
						ValidateLifetime = false,
						ValidateIssuerSigningKey = false,
						ValidateTokenReplay = false,
						IssuerSigningKey = Constants.JwtToken.SECURITY_KEY,
					};

					// We have to hook the OnMessageReceived event in order to
					// allow the JWT authentication handler to read the access
					// token from the query string when a WebSocket or 
					// Server-Sent Events request comes in.
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							string accessToken = context.Request.Query["access_token"].ToString();

							Console.WriteLine($"accessToken : {accessToken}");
							// If the request is for our hub...
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken))
							{
								// Read the token out of the query string
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						},
						OnAuthenticationFailed = context =>
						{
							context.NoResult();
							context.Response.StatusCode = 500;
							context.Response.ContentType = "text/plain";
							context.Response.WriteAsync(context.Exception.ToString()).Wait();
							Console.WriteLine($"context.Exception.ToString() : {context.Exception.ToString()}");
							return Task.CompletedTask;
						}
					};
				});

			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});


			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			services.AddControllers();
			//.AddAzureSignalR("");

			services.AddSingleton<ILoggerFactory, LoggerFactory>();
			services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddHttpContextAccessor();

			//services.AddTransient<IBalanceQueries, BalanceQueries>();

			// databases
			services.AddTransient<IUserQueries, UserQueries>();
			services.AddTransient<ILeaderboardWeeklyQueries, LeaderboardWeeklyQueries>();

			// managers objects
			services.AddTransient<ISignUpManager, SignUpManager>();
			services.AddTransient<IJwtTokenManager, JwtTokenManager>();
		}


		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> _logger, IServiceProvider serviceProvider, IClusterClient grainsClient)
		{
			app.UseCors("AllowOrigin");

			logger = _logger;


			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCookiePolicy();

			app.UseRouting();

			app.Use((context, next) =>
			{
				context.Request.EnableBuffering(); // calls EnableRewind() `https://github.com/dotnet/aspnetcore/blob/4ef204e13b88c0734e0e94a1cc4c0ef05f40849e/src/Http/Http/src/Extensions/HttpRequestRewindExtensions.cs#L23`
				return next();
			});

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.MapControllerRoute(
				//    name: "refer",
				//    pattern: "{controller=Refer}/{action=Index}/{id?}");
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});


			app.UseDefaultFiles();
			StaticFileOptions option = new StaticFileOptions();
			//FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
			//contentTypeProvider.Mappings.Add(".assetbundle", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".mem", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".data", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".memgz", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".datagz", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".unity3dgz", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".unityweb", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".unitypackage", "application/octet-stream");
			//contentTypeProvider.Mappings.Add(".jsgz", "application/x-javascript; charset=UTF-8");
			//option.ContentTypeProvider = contentTypeProvider;

			option.DefaultContentType = "application/octet-stream";
			option.ServeUnknownFileTypes = true;
			option.OnPrepareResponse += content =>
			{
				if (content.File.Name.EndsWith(".gz"))
				{
					content.Context.Response.Headers["Content-Encoding"] = "gzip";
				}

				const int durationInSeconds = 60 * 60 * 24;
				content.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
			};

			app.UseStaticFiles(option);
			app.Run(async (context) =>
			{

			});

			await grainsClient.GetGrain<IUniqueServiceGrain>(Guid.Empty).StartReminders();
		}
	}
}
