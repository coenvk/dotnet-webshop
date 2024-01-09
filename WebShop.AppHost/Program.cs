using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Hosting;

namespace WebShop.AppHost;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<Projects.WebShop_ApiGateway_Ocelot>("api-gateway")
            .WithLaunchProfile("default")
            .WithServiceBinding(hostPort: 8070, scheme: "https")
            .WithServiceBinding(hostPort: 8071, scheme: "http");

        builder.AddProject<Projects.WebShop_Catalog_Api>("catalog-api")
            .WithServiceBinding(hostPort: 8010, scheme: "https")
            .WithServiceBinding(hostPort: 8011, scheme: "http");

        builder.AddProject<Projects.WebShop_Inventory_Api>("inventory-api")
            .WithServiceBinding(hostPort: 8020, scheme: "https")
            .WithServiceBinding(hostPort: 8021, scheme: "http");

        builder.AddProject<Projects.WebShop_Order_Api>("order-api")
            .WithServiceBinding(hostPort: 8030, scheme: "https")
            .WithServiceBinding(hostPort: 8031, scheme: "http");

        builder.AddProject<Projects.WebShop_Payment_Api>("payment-api")
            .WithServiceBinding(hostPort: 8040, scheme: "https")
            .WithServiceBinding(hostPort: 8041, scheme: "http");

        if (builder.Environment.IsDevelopment())
        {
            var dcpHook = builder.Services.FirstOrDefault(sd
                => sd.ImplementationType != null
                   && sd.ServiceType.IsAssignableTo(typeof(IDistributedApplicationLifecycleHook))
                   && sd.ImplementationType.Name.Equals("DcpDistributedApplicationLifecycleHook"));

            if (dcpHook != null)
            {
                builder.Services.Remove(dcpHook);
            }
        }

        var app = builder.Build();

        app.Run();
    }
}