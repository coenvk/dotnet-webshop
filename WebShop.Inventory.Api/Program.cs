using System;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Toolkit.Endpoints.MediatR;
using Toolkit.Repository.EntityFrameworkCore;
using Toolkit.Repository.EntityFrameworkCore.Abstractions;
using Toolkit.Sagas.Rebus.DependencyInjection;
using WebShop.Catalog.Contracts.Events;
using WebShop.Inventory.Api.Domain;
using WebShop.Inventory.Api.Features.UpdateStock;
using WebShop.Inventory.Api.Infrastructure;
using WebShop.Inventory.Contracts.Commands;
using WebShop.Inventory.Contracts.Dtos;
using WebShop.Inventory.Contracts.Events;
using WebShop.Order.Contracts.Commands;

namespace WebShop.Inventory.Api;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddControllers();
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        builder.Services.AddMediatREndpoints(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddRebus(options =>
        {
            options
                .Routing(r => r.TypeBased()
                    .MapAssemblyOf(typeof(StockDto), "inventory-queue"))
                .Transport(t => t.UseRabbitMq(builder.Configuration.GetConnectionString("MessageBroker"),
                    "inventory-queue"));

            return options;
        }, onCreated: async bus =>
        {
            await bus.Subscribe<ProductCreated>();
            await bus.Subscribe<ProductDeleted>();
            await bus.Subscribe<CancelStock>();
            await bus.Subscribe<UpdateStock>();
        });

        builder.Services.AutoRegisterHandlersFromAssembly(typeof(Program).Assembly);

        builder.Services.AddHealthChecks();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration.GetConnectionString("Database")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'Database' not found.");

        builder.Services
            .AddDbContext<DbContext, ApplicationDbContext>(options => options.UseSqlite(connectionString));

        builder.Services.AddAutoMapper(options =>
        {
            options.CreateMap<Stock, StockDto>()
                .ForMember(dest => dest.ProductId, o => o.MapFrom(src => src.Id))
                .ReverseMap();
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<StockRepository>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(typeof(DbContext));
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMediatR();
        });

        app.MapHealthChecks("_health");

        app.Run();
    }
}