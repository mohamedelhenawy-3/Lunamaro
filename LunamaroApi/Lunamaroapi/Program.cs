using FluentValidation;
using FluentValidation.AspNetCore;
using Lunamaroapi.BackgroundServices;
using Lunamaroapi.Data;
using Lunamaroapi.Helper;
using Lunamaroapi.Helper.EmailSetting;
using Lunamaroapi.Middlwares;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Implementations;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Implements;
using Lunamaroapi.Services.Interfaces;
using Lunamaroapi.Validators.ItemValidators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Stripe;
using System;
using System.Text;
using System.Text.Json.Serialization;
using CategoryService = Lunamaroapi.Services.Implements.CategoryService;
using ItemService = Lunamaroapi.Services.Implements.ItemService;
using TokenService = Lunamaroapi.Services.Implements.TokenService;

namespace Lunamaroapi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Logger Configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secretkey"];

      
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://lunamarofrontend.z1.web.core.windows.net",
                            "http://lunamaro.runasp.net",   
                            "https://lunamaro.runasp.net" ,
                            "https://lunamaro.netlify.app",
                            "https://lunamar.netlify.app" 

                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); 
                });
            });

            builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);


            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });



            builder.Services.AddDbContext<AppDBContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
           sqlServerOptionsAction: sqlOptions =>
           {
               sqlOptions.EnableRetryOnFailure(
                   maxRetryCount: 5,
                   maxRetryDelay: TimeSpan.FromSeconds(30),
                   errorNumbersToAdd: null);
           }));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

            // 5. Dependency Injection (Repositories & Services)
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRefreshToken, RefrshTokenRepoitory>();
            builder.Services.AddScoped<IOffersRepository, OfferRepository>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IImageServices, ImageService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IUserCart, UserCartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ITable, TableServices>();
            builder.Services.AddScoped<IReservation, ReservationServices>();
            builder.Services.AddScoped<IDashboard, DashboardServices>();
            builder.Services.AddScoped<IReview, ReviewsService>();
            builder.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
            builder.Services.AddScoped<IRecommendationService, RecommendationService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();

            // 6. Background Services
            builder.Services.AddHostedService<EmailBackgroundService>();
            builder.Services.AddHostedService<StockCleanupWorker>();

            // 7. Middlewares & Other Helpers
            builder.Services.AddTransient<GlobalExceptionMiddleware>();
            builder.Services.AddSingleton<SmsService>();
            builder.Services.Configure<ESetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddScoped<ISocialAuthService, SocialAuthService>();
            builder.Services.AddScoped<IIdentityService, IdentityService>();
            builder.Services.AddScoped<IAiChatService, AiChatService>();
            // 8. Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ItemDTOValidator>();

            // 9. Controllers & JSON
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // 10. Authentication & JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            // 11. Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT Token"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            // 12. App Build and Lifecycle
            try
            {
                Log.Information("Starting Lunamaro Web API...");
                var app = builder.Build();

                // --- MIGRATION SAFE SEEDING START ---
                using (var scope = app.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                        // Only try to seed if the DB is actually accessible
                        if (context.Database.CanConnect())
                        {
                            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                            await SeedRolesAsync(roleManager);
                        }
                    }
                    catch (Exception)
                    {
                        Log.Warning("Seeding skipped: Database or Tables not ready yet.");
                    }
                }
                // --- MIGRATION SAFE SEEDING END ---

                app.UseMiddleware<GlobalExceptionMiddleware>();

                // Always Enable Swagger for MonsterASP (helpful for testing)
                app.UseSwagger();
                app.UseSwaggerUI();
         app.UseSerilogRequestLogging();
                app.UseResponseCompression();
                app.MapFallbackToFile("index.html");
                app.UseCors("AllowAll");
                app.UseHttpsRedirection();
                // Add this before app.UseStaticFiles()
                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".webmanifest"] = "application/manifest+json";

                app.UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = provider,
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=604800, immutable";
                    }
                }); app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                Console.WriteLine("--> API is running and ready for requests.");
                await app.RunAsync();
            }
            catch (Exception ex) when (ex is not HostAbortedException)
            {
                Log.Fatal(ex, "The application failed to start correctly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}