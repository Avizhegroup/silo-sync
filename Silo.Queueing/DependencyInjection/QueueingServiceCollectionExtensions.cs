namespace Silo.Queueing;

/// <summary>Registers the reusable queue infrastructure.</summary>
public static class QueueingServiceCollectionExtensions
{
    /// <summary>
    /// Registers queue persistence, RabbitMQ transport, saga processing, and workflow services.
    /// Handler implementations must be registered by the consuming application.
    /// </summary>
    public static IServiceCollection AddCompanyQueueing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var queueSection = configuration.GetSection(CompanyQueueingOptions.SectionName);
        services.AddOptions<CompanyQueueingOptions>()
            .Bind(queueSection)
            .Validate(ValidateSettings, "CompanyQueueing settings are incomplete or invalid.")
            .ValidateOnStart();

        var settings = queueSection.Get<CompanyQueueingOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{CompanyQueueingOptions.SectionName}' is required.");

        services.AddDbContextFactory<QueueDbContext>(db =>
            db.UseSqlServer(
                settings.Database.ConnectionString,
                sql => sql.MigrationsAssembly(typeof(QueueDbContext).Assembly.GetName().Name)));

        services.AddScoped<IQueueWorkflowService, QueueWorkflowService>();
        services.AddScoped<IQueueDatabaseMigrator, QueueDatabaseMigrator>();
        services.AddScoped<QueueTaskHandlerResolver>();
        services.AddHostedService<QueueHandlerValidationService>();
        services.AddHostedService<QueueRecoveryService>();

        services.AddMassTransit(registration =>
        {
            registration.AddConsumer<QueueTaskConsumer>()
                .Endpoint(endpoint =>
                {
                    endpoint.Name = QueueTaskConsumer.EndpointName;
                    endpoint.ConcurrentMessageLimit = settings.Processing.ConcurrentWorkflows;
                });
            registration.AddConsumer<QueueTaskFaultConsumer>();

            registration.AddSagaStateMachine<QueueWorkflowStateMachine, QueueWorkflowSaga>()
                .EntityFrameworkRepository(repository =>
                {
                    repository.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    repository.AddDbContext<DbContext, QueueDbContext>((_, builder) =>
                        builder.UseSqlServer(settings.Database.ConnectionString));
                    repository.UseSqlServer();
                });

            registration.AddEntityFrameworkOutbox<QueueDbContext>(outbox =>
            {
                outbox.UseSqlServer();
                outbox.UseBusOutbox();
            });

            registration.AddConfigureEndpointsCallback((context, endpointName, endpoint) =>
            {
                if (!string.Equals(
                        endpointName,
                        QueueTaskConsumer.EndpointName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    endpoint.UseEntityFrameworkOutbox<QueueDbContext>(context);
                }

                if (settings.Processing.MaxRetryCount > 0)
                {
                    endpoint.UseDelayedRedelivery(retry =>
                        retry.Exponential(
                            settings.Processing.MaxRetryCount,
                            TimeSpan.FromSeconds(settings.Processing.InitialRetryDelaySeconds),
                            TimeSpan.FromSeconds(settings.Processing.MaxRetryDelaySeconds),
                            TimeSpan.FromSeconds(settings.Processing.InitialRetryDelaySeconds)));
                }

                endpoint.UseMessageRetry(retry => retry.Immediate(2));
            });

            registration.UsingRabbitMq((context, bus) =>
            {
                bus.Host(
                    settings.RabbitMq.Host,
                    settings.RabbitMq.VirtualHost,
                    host =>
                    {
                        host.Username(settings.RabbitMq.Username);
                        host.Password(settings.RabbitMq.Password);
                    });

                bus.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static bool ValidateSettings(CompanyQueueingOptions options)
    {
        var processing = options.Processing;
        return !string.IsNullOrWhiteSpace(options.RabbitMq.Host) &&
               !string.IsNullOrWhiteSpace(options.RabbitMq.VirtualHost) &&
               !string.IsNullOrWhiteSpace(options.RabbitMq.Username) &&
               !string.IsNullOrWhiteSpace(options.RabbitMq.Password) &&
               !string.IsNullOrWhiteSpace(options.Database.ConnectionString) &&
               processing.MaxRetryCount is >= 0 and <= 100 &&
               processing.InitialRetryDelaySeconds is >= 1 and <= 86400 &&
               processing.MaxRetryDelaySeconds is >= 1 and <= 604800 &&
               processing.InitialRetryDelaySeconds <= processing.MaxRetryDelaySeconds &&
               processing.RequestTimeoutSeconds is >= 1 and <= 86400 &&
               processing.ConcurrentWorkflows is >= 1 and <= 1000;
    }
}
