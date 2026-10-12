namespace Silo.Queueing;

/// <summary>Settings bound from the CompanyQueueing configuration section.</summary>
public sealed class CompanyQueueingOptions
{
    public const string SectionName = "CompanyQueueing";

    public RabbitMqQueueOptions RabbitMq { get; set; } = new();
    public QueueDatabaseOptions Database { get; set; } = new();
    public QueueProcessingOptions Processing { get; set; } = new();
}

public sealed class RabbitMqQueueOptions
{
    [Required]
    public string Host { get; set; } = string.Empty;

    public string VirtualHost { get; set; } = "/";

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class QueueDatabaseOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}

public sealed class QueueProcessingOptions
{
    [Range(0, 100)]
    public int MaxRetryCount { get; set; } = 5;

    [Range(1, 86400)]
    public int InitialRetryDelaySeconds { get; set; } = 5;

    [Range(1, 604800)]
    public int MaxRetryDelaySeconds { get; set; } = 300;

    [Range(1, 86400)]
    public int RequestTimeoutSeconds { get; set; } = 60;

    [Range(1, 1000)]
    public int ConcurrentWorkflows { get; set; } = 4;
}
