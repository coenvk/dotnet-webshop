using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace WebShop.ApiGateway.Ocelot;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        var configuration = builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("ocelot.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment}.json", true, true)
            .AddJsonFile($"ocelot.{builder.Environment}.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddOcelot(configuration);
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors();

        app.UseOcelot().Wait();
        app.Run();
    }
}