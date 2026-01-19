
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NETMockServer.Data;
using NETMockServer.Entities;
using NETMockServer.Extensions;
using NETMockServer.Repositories;
using NETMockServer.Repositories.Interfaces;
using NETMockServer.Seed;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1" }));

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

        // Register default generic faker and specific fakers (specific override default)
        builder.Services.AddTransient(typeof(IEntityFaker<>), typeof(DefaultEntityFaker<>));
        builder.Services.AddTransient<IEntityFaker<Product>, ProductFaker>();
        builder.Services.AddTransient<IEntityFaker<Customer>, CustomerFaker>();

        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<FakeDataSeeder>();

        var app = builder.Build();
        app.UseHttpsRedirection();

        var appName = app.Environment.ApplicationName;
        var swaggerJson = "/swagger/v1/swagger.json";
        var swaggerTitle = string.IsNullOrWhiteSpace(appName) ? "API v1" : $"{appName} v1";

        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint(swaggerJson, swaggerTitle));

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();

            var seeder = scope.ServiceProvider.GetRequiredService<FakeDataSeeder>();

            // Seed 30 products and 20 customers if empty
            await seeder.EnsureSeedAsync<Product>(30);
            await seeder.EnsureSeedAsync<Customer>(20);
        }

        app.UseRouting();

        app.MapEntityEndpoints<Product>("Products");
        app.MapEntityEndpoints<Customer>("Customers");

        app.Run();
    }
}