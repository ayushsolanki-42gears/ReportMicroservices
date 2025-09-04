using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportMicroservice.Messaging;
using ReportMicroservice.Reports;
using ReportMicroservice.Storage;
using ReportMicroservice.Workers;
using ReportMicroservice.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;

        // ===== Options =====
        services.Configure<MongoOptions>(config.GetSection("Mongo"));
        services.Configure<AwsOptions>(config.GetSection("AWS"));
        services.Configure<KafkaOptions>(config.GetSection("Kafka"));

        // ===== Data =====
        services.AddSingleton<IMongoRepository, MongoRepository>();

        // ===== Reports =====
        services.AddSingleton<IReportGenerator, CsvSalesReportGenerator>();

        // ===== Storage (AWS S3) =====
        services.AddSingleton<IS3Storage, S3Storage>();

        // ===== Messaging (Kafka) =====
        services.AddSingleton<IReportRequestConsumer, ReportRequestConsumer>();
        services.AddSingleton<IReportEventProducer, ReportEventProducer>();

        // ===== Worker =====
        services.AddHostedService<ReportWorker>();
    })
    .Build();

await host.RunAsync();
