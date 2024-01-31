using System.Reflection;

using AuctionApp.Application.Features.Invoices.CreateInvoice;
using AuctionApp.Domain.Settings;

using MassTransit;

using Microsoft.Extensions.Options;

namespace AuctionApp.Host.Configuration;

public static class RabbitMqConfiguration
{
    public static void AddMessaging(this IServiceCollection services)
    {
        var rabbitMqSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<RabbitMqSettings>>()?.Value;

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            var assembly = Assembly.GetAssembly(typeof(CreateInvoiceConsumer));
            x.AddConsumers(assembly);
            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddActivities(assembly);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == Environments.Development)
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqSettings?.Host, "/",
                        h =>
                        {
                            h.Username(rabbitMqSettings!.Username);
                            h.Password(rabbitMqSettings.Password);
                        });

                    cfg.ConfigureEndpoints(context);
                });
            }

            // else
            // {
            //     x.UsingRabbitMq((context, cfg) =>
            //     {
            //         cfg.Host(new Uri(rabbitMqSettings?.Host!), "/",
            //             h =>
            //             {
            //                 h.Username(rabbitMqSettings!.Username);
            //                 h.Password(rabbitMqSettings!.Password);
            //             });
            //
            //         cfg.ConfigureEndpoints(context);
            //     });
            // }
        });
    }
}