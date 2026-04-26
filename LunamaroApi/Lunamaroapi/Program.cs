// ... (keep your namespaces)

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

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Information() 
             .WriteTo.Console()         
             .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
             .CreateLogger();

            builder.Host.UseSerilog();
            // CORRECT
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secretkey"];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "https://lunamarofrontend.z1.web.core.windows.net"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(); // Add this line
        }));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();

            // Repositories & Services
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IRefreshToken, RefrshTokenRepoitory>();
            builder.Services.AddScoped<IOffersRepository, OfferRepository>();
            builder.Services.AddScoped<IPricingService, PricingService>();
            builder.Services.AddScoped<JwtTokenGenerator>();
            builder.Services.AddScoped<IImageServices, ImageService>();
            builder.Services.AddSingleton<IImageServices>(sp =>
    new ImageService(builder.Configuration));
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

            // Background Services
           builder.Services.AddHostedService<EmailBackgroundService>();
            // If you uncomment this, make sure StockCleanupWorker uses 'await Task.Yield()' at the start
            builder.Services.AddHostedService<StockCleanupWorker>();

            builder.Services.AddTransient<GlobalExceptionMiddleware>();
            builder.Services.AddSingleton<SmsService>();
            builder.Services.Configure<ESetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSingleton<EmailService>();

            // Validation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ItemDTOValidator>();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

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


            try
            {
                Log.Information("Starting Lunamaro Web API...");
                var app = builder.Build();
                app.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Error = ex.Message,
                            Inner = ex.InnerException?.Message,
                            Stack = ex.StackTrace
                        });
                    }
                });
                // 3. Seed Roles (Moved after app.Build but before app.Run)
                using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRolesAsync(roleManager);
            }

            // 4. Middleware Pipeline
            app.UseMiddleware<GlobalExceptionMiddleware>();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseSerilogRequestLogging();
                app.UseCors("AllowAll");
                app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            Console.WriteLine("--> API is running and ready for requests.");
            await app.RunAsync();
            }
            catch (Exception ex)
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