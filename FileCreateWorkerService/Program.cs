using FileCreateWorkerService;
using FileCreateWorkerService.Models;
using FileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        IConfiguration Configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddSingleton<RabbitMQClientService>();

        services.AddDbContext<AdventureWorks2019Context>(opt =>
        {
            opt.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
        });

        services.AddSingleton(sp => new ConnectionFactory()
        {

            Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")),
            DispatchConsumersAsync = true
        });

        services.AddHostedService<Worker>();
    }).Build();



host.Run();