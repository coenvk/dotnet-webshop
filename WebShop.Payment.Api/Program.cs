using System.Text.Json;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Toolkit.Endpoints.MediatR;
using Toolkit.Sagas.Rebus.DependencyInjection;
using WebShop.Order.Contracts.Commands;
using WebShop.Order.Contracts.Dtos;
using WebShop.Payment.Api.Features.ProcessPayment;
using WebShop.Payment.Contracts.Commands;
using WebShop.Payment.Contracts.Events;

namespace WebShop.Payment.Api;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

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
                    .MapAssemblyOf(typeof(PaymentCompleted), "payment-queue"))
                .Transport(t => t.UseRabbitMq(builder.Configuration.GetConnectionString("MessageBroker"),
                    "payment-queue"));

            return options;
        }, onCreated: async bus =>
        {
            await bus.Subscribe<ProcessPayment>();
        });

        builder.Services.AutoRegisterHandlersFromAssembly(typeof(Program).Assembly);

        builder.Services.AddHealthChecks();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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
            endpoints.MapMediatR();
        });

        app.MapHealthChecks("_health");

        app.Run();
    }
}