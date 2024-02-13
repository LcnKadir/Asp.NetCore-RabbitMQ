using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AdventureWorks2012Context>(opt =>
        {
            opt.UseSqlServer(context.Configuration.GetConnectionString("SqlServer"));
        });

        services.AddScoped<RabbitMQClientService>();

        IConfiguration Configuration = context.Configuration;
        services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });


        services.AddHostedService<Worker>();
    })
    .Build();



host.Run();