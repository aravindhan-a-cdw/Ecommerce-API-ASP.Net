
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTO;
using EcommerceAPI.Repository;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
using EcommerceAPI.Utilities.IUtilities;
using EcommerceAPI.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using StackExchange.Redis;

namespace EcommerceAPI;

public class Program
{

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Add services to the container and To Convert Enum to String instead of Integers in the Swagger UI
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();

        // User Authentication with Custom User Model
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        // Get Values from Configuration file
        var JWT_SECRET = builder.Configuration.GetValue<string>(Constants.JWT_SECRET_CONFIGURATION_KEY);
        var REDIS_CONNECTION_STRING = builder.Configuration.GetConnectionString(Constants.REDIS_CONFIGURATION_KEY);
        var POSTGRES_CONNECTION_STRING = builder.Configuration.GetConnectionString(Constants.POSTGRES_CONFIGURATION_KEY);

        if(JWT_SECRET == null || REDIS_CONNECTION_STRING == null || POSTGRES_CONNECTION_STRING == null)
        {
            throw new Exception(Constants.Messages.CONFIGURATION_KEY_NOT_SET);
        }

        // Add authentication and authorization middlewares
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWT_SECRET)),
                ValidateAudience = false,
                ValidateIssuer = false
            };
            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new JWTValidator(ConnectionMultiplexer.Connect(REDIS_CONNECTION_STRING)));
        });

        builder.Services.AddAuthorization();

        // Cache Response
        builder.Services.AddResponseCaching();
        builder.Services.AddMemoryCache();

        // Swagger Config for JWT Security
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
            {
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = Constants.Swagger.JWT_SECURITY_DESCRIPTION
            });
            // Filter to add Swagger lock only to Routes that require Authorization
            options.OperationFilter<SecurityFilter>();
        });

        // Add database Context
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(POSTGRES_CONNECTION_STRING);
        });

        // Add Redis Connection Dependency
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(REDIS_CONNECTION_STRING));

        // Utility Injection
        builder.Services.AddScoped<ICustomerUtility, CustomerUtility>();
        builder.Services.AddScoped<IRequestUtility, RequestUtility>();

        // Add Custom Services for Dependency Injection
        // Repository Injection
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
        builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
        builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();
        builder.Services.AddScoped<IRepository<Inventory>, Repository<Inventory>>();
        builder.Services.AddScoped<IRepository<Models.Order>, Repository<Models.Order>>();
        builder.Services.AddScoped<IRepository<OrderItem>, Repository<OrderItem>>();

        // Service Injection
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        
        // Add Automapper Service
        builder.Services.AddAutoMapper(typeof(MappingConfig));

        // Add ContextAccessor to access headers in logout route
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<CartCleanJob>(opts => opts.WithIdentity("Cart"));

            q.AddTrigger(opts => opts
                .ForJob("Cart")
                .WithIdentity("Cart-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(50)
                    .RepeatForever())
            );
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

        // Exception Handler
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionFeature == null) return;

                var exception = exceptionFeature.Error;
                if (exception is BadHttpRequestException)
                {
                    var obj = (BadHttpRequestException)exception;
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = obj.StatusCode;
                    var response = new APIResponseDTO(obj.StatusCode, obj.Message);
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync(Constants.Messages.UNHANDLED_EXCEPTION);
                }
            });
        });

        app.MapControllers();
        app.Run();
    }
}

