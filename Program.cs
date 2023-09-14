
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
using EcommerceAPI.Utilities;
using Quartz;
using Quartz.Impl;
using static Quartz.Logging.OperationName;
using System.Collections.Specialized;

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
        builder.Services.AddScoped<CustomerUtility>();
        builder.Services.AddScoped<RequestsUtility>();

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
                //This Cron interval can be described as "run every minute" (when second is zero)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(50)
                    .RepeatForever())
            );

            //var job = JobBuilder.Create<CartCleanJob>()
            //    .WithIdentity("myJob", "group1")
            //    .Build();
            //var trigger = TriggerBuilder.Create()
            //    .WithIdentity("myTrigger", "group1")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(30)
            //        .RepeatForever());
            
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        //var serviceProvider = serviceCollection.BuildServiceProvider();

        //var props = new NameValueCollection
        //    {
        //        { "quartz.serializer.type", "binary" }
        //    };
        //var factory = new StdSchedulerFactory(props);
        //var sched = await factory.GetScheduler();
        //await sched.Start();
        //var job = JobBuilder.Create<DemoJob>()
        //    .WithIdentity("myJob", "group1")
        //    .Build();
        //var trigger = TriggerBuilder.Create()
        //    .WithIdentity("myTrigger", "group1")
        //    .StartNow()
        //    .WithSimpleSchedule(x => x
        //        .WithIntervalInSeconds(50)
        //        .RepeatForever())
        //.Build();
        //await sched.ScheduleJob(job, trigger);
        //sched.JobFactory = new DemoJobFactory(serviceProvider);
        //sched.JobFactory = new DemoJobFactory(serviceProvider);

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

        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
        IScheduler scheduler = schedulerFactory.GetScheduler().Result;

//        var props = new NameValueCollection
//{
//    { "quartz.serializer.type", "binary" }
//};
//        var factory = new StdSchedulerFactory(props);
//        var sched = await factory.GetScheduler();
//        await sched.Start();
//        var job = JobBuilder.Create<CartResetJob>()
//            .WithIdentity("myJob", "group1")
//            .Build();
//        var trigger = TriggerBuilder.Create()
//            .WithIdentity("myTrigger", "group1")
//            .StartNow()
//            .WithSimpleSchedule(x => x
//                .WithIntervalInSeconds(5)
//                .RepeatForever())
//        .Build();
//        await sched.ScheduleJob(job, trigger);

        //        // Define a job and trigger
        //        IJobDetail job = JobBuilder.Create<CartResetJob>().Build();
        //        ITrigger trigger = TriggerBuilder.Create()
        //            .WithIdentity("myTrigger")
        //            .StartNow()
        //            .WithSimpleSchedule(x => x
        //                .WithIntervalInSeconds(5)
        //                .RepeatForever())
        //            .Build();

        //        // Schedule the job with the trigger
        //        scheduler.ScheduleJob(job, trigger);

        //        // Start the scheduler
        //        scheduler.Start();

        app.Run();
    }
}

