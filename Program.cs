
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
using EcommerceAPI.utils;
using StackExchange.Redis;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Diagnostics;
using EcommerceAPI.Models.DTO;
using System.Text.Json;

namespace EcommerceAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Add services to the container and To Convert Enum to String instead of Integers in the Swagger UI
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();

        // User Authentication with Custom User Model
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddTokenProvider<DataProtectorTokenProvider<User>>("Demo");

        // Add JWT Authentication
        var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateAudience = false,
                ValidateIssuer = false
            };
            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(new JWTValidator(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisCache"))));
        });

        builder.Services.AddAuthorization();

        // Cache Response
        builder.Services.AddResponseCaching();

        // Swagger Config for JWT Security
        builder.Services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Authorization using Bearer Scheme in Header"
            });
            // Filter to add Swagger lock only to Routes that require Authorization
            options.OperationFilter<SecurityFilter>();
        });

        // Add database Context
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresqlConnection"));
        });

        // Add Redis Connection Dependency
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisCache")));

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
                var exception = context.Features.Get<IExceptionHandlerFeature>().Error;
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
                    await context.Response.WriteAsync("Unknown Error");
                }
            });
        });

        app.MapControllers();

        app.Run();
    }
}

