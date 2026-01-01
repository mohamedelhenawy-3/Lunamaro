using FluentValidation;
using FluentValidation.AspNetCore;
using Lunamaroapi.Data;
using Lunamaroapi.Helper;
using Lunamaroapi.Helper.EmailSetting;
using Lunamaroapi.Middlwares;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Implementations;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Implements;
using Lunamaroapi.Services.Interfaces;
using Lunamaroapi.Validators.CategoryValidator;
using Lunamaroapi.Validators.ItemValidators;
using Lunamaroapi.Validators.OrderValidator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Text;
using System.Text.Json.Serialization; // <-- Add this at the top

namespace Lunamaroapi
{
    public class Program
    {
        public static async Task Main(string[] args) // Make Main async
        {
            var builder = WebApplication.CreateBuilder(args);
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // 1. Add CORS BEFORE Build
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
policy =>
{
    policy.SetIsOriginAllowed(origin => true)
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();

});

                //policy =>
                //{
                //    policy.WithOrigins("http://localhost:4200", "https://localhost:4200") // Angular frontend URL
                //          .AllowAnyHeader()
                //          .AllowAnyMethod()
                //          .AllowCredentials();
                //});

            });

            // 2. Add other services
            builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders();







            builder.Services.AddScoped<IAuthServices,Services.Implements.AuthServices>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITokenService,Services.Implements.TokenService>();
            builder.Services.AddScoped<IRefreshToken, RefrshTokenRepoitory>();

            //builder.Services.AddScoped<IAuth,Services>
            builder.Services.AddScoped<JwtTokenGenerator>();
            builder.Services.AddScoped<IImageServices, ImageService>();
            builder.Services.AddScoped<ICategoryService,Services.Implements.CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            //builder.Services.AddScoped<IItem, ItemService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IItemService, Services.Implements.ItemService>();

            builder.Services.AddScoped<IUserCart, UserCartService>();
            builder.Services.AddScoped<IOrder, OrderServices>();
            builder.Services.AddScoped<ITable, TableServices>();
            builder.Services.AddScoped<IReservation, ReservationServices>();
            builder.Services.AddScoped<IDashboard, DashboardServices>();
            builder.Services.AddScoped<IReview, ReviewsService>();
            // Inside the service configuration block in Program.cs
            builder.Services.AddTransient<GlobalExceptionMiddleware>();
            builder.Services.AddSingleton<SmsService>();
            builder.Services.Configure<ESetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSingleton<EmailService>();

            //fluentApiValidtor
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ItemDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateItemDTOValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateCategpryValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();



            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This converts enums (like ReservationStatus) to strings in JSON responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // 3. Build the app
            var app = builder.Build();

            // NEW - Works because the framework can build the middleware instance at startup.
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // 4. Create roles using scope (safe here, no service modification)
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Admin", "Customer" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // 5. Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAngularApp"); // Use CORS middleware here, AFTER Build
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(); // Already serves files in wwwroot

         

            app.MapControllers();

            app.Run();

        }
    }
}
