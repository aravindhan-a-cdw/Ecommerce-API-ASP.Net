
using System.Text.Json.Serialization;
using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Repository;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using EcommerceAPI.utils;

namespace EcommerceAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Add services to the container and To Convert Enum to String instead of Integers in the Swagger UI
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // User Authentication with Custom User Model
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddTokenProvider<DataProtectorTokenProvider<User>>("Demo");

        // Add JWT Authentication
        var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddResponseCaching();

        // Swagger Config for JWT Security
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Authorization using Bearer Scheme in Header"
            });

            //options.AddSecurityRequirement(new OpenApiSecurityRequirement{
            //    {
            //        new OpenApiSecurityScheme{
            //            Reference = new OpenApiReference{
            //                Id = "Bearer", //The name of the previously defined security scheme.
            //                Type = ReferenceType.SecurityScheme
            //            },
            //            Scheme = "oauth2",
            //            Name = "Bearer",
            //        },new List<string>()
            //    }
            //});
            //options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            BearerFormat = "JWT",
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = JwtBearerDefaults.AuthenticationScheme
            //            },
            //            Scheme = "oauth2",
            //            Name = "Bearer",
            //            In = ParameterLocation.Header,
            //            Type = SecuritySchemeType.Http
            //        },
            //        new List<string>()
            //    }
            //});

            options.OperationFilter<SecurityFilter>();
        });

        // Add database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresqlConnection"));
        });

        // Add Custom Services for Dependency Injection
        // UserRepository Injection
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<Repository<Product>, ProductRepository>();
        builder.Services.AddScoped<Repository<Category>, CategoryRepository>();
        builder.Services.AddScoped<Repository<Cart>, CartRepository>();
        builder.Services.AddScoped<Repository<User>, UserRepository>();
        builder.Services.AddScoped<InventoryRepository, InventoryRepository>();
        builder.Services.AddScoped<Repository<Order>, OrderRepository>();
        builder.Services.AddScoped<Repository<OrderItem>, OrderItemsRepository>();

        // Add Automapper Service
        builder.Services.AddAutoMapper(typeof(MappingConfig));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(builder =>
        {
            builder.Run(context =>
            {
                context.Response.WriteAsync("Error occurred");
                return Task.CompletedTask;
            });
        });

        app.MapControllers();

        app.Run();
    }
}

