
using System.Text.Json.Serialization;
using EcommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Repository;
using EcommerceAPI.Models;

namespace EcommerceAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // To Convert Enum to String instead of Integers in the Swagger UI
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add database
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresqlConnection"));
        });

        // Add Custom Services for Dependency Injection
        // UserRepository Injection
        builder.Services.AddScoped<IRepository<User>, UserRepository>();
        // RoleRepository Injection
        builder.Services.AddScoped<IRepository<Role>, RoleRepository>();

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

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

