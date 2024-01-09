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
using WebShop.Inventory.Contracts.Commands;
using WebShop.Inventory.Contracts.Events;
using WebShop.Order.Api.Domain;
using WebShop.Order.Api.Features.CompleteOrder;
using WebShop.Order.Api.Features.CreateOrder;
using WebShop.Order.Api.Infrastructure;
using WebShop.Order.Contracts.Commands;
using WebShop.Order.Contracts.Dtos;
using WebShop.Order.Contracts.Events;
using WebShop.Payment.Contracts.Commands;
using WebShop.Payment.Contracts.Events;

namespace WebShop.Order.Api;

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
                    .MapAssemblyOf(typeof(OrderDto), "order-queue")
                    .Map(typeof(UpdateStock), "inventory-queue")
                    .Map(typeof(CancelStock), "inventory-queue")
                    .Map(typeof(ProcessPayment), "payment-queue"))
                .Transport(t => t.UseRabbitMq(builder.Configuration.GetConnectionString("MessageBroker"),
                    "order-queue"));

            options.Sagas(s => s.StoreInMemory());

            return options;
        }, onCreated: async bus =>
        {
            await bus.Subscribe<CompleteOrder>();
            await bus.Subscribe<CancelOrder>();
            await bus.Subscribe<StockUpdated>();
            await bus.Subscribe<StockUpdateFailed>();
            await bus.Subscribe<PaymentCompleted>();
            await bus.Subscribe<PaymentFailed>();
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
            options.CreateMap<Domain.Order, OrderDto>()
                .ForMember(dest => dest.OrderId, o => o.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderLines, o => o.MapFrom(src => src.OrderLines))
                .ReverseMap();
            options.CreateMap<CreateOrder, Domain.Order>();
            options.CreateMap<OrderLine, OrderLineDto>()
                .ForSourceMember(src => src.Order, o => o.DoNotValidate())
                .ReverseMap();
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<OrderRepository>();

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