namespace Silo.Queueing;

/// <summary>Settings bound from the CompanyQueueing configuration section.</summary>
public sealed class CompanyQueueingOptions
{
    /// <summary>Configuration section bound by <c>AddCompanyQueueing</c>.</summary>
    public const string SectionName = "CompanyQueueing";

    /// <summary>RabbitMQ connection settings.</summary>
    public RabbitMqQueueOptions RabbitMq { get; set; } = new();

    /// <summary>Independent SQL Server queue database settings.</summary>
    public QueueDatabaseOptions Database { get; set; } = new();

    /// <summary>Workflow execution, timeout, retry, and concurrency settings.</summary>
    public QueueProcessingOptions Processing { get; set; } = new();
}

/// <summary>RabbitMQ endpoint and credentials.</summary>
public sealed class RabbitMqQueueOptions
{
    /// <summary>RabbitMQ host name.</summary>
    [Required]
    public string Host { get; set; } = string.Empty;

    /// <summary>RabbitMQ virtual host.</summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>RabbitMQ user name, supplied through configuration or a secret provider.</summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>RabbitMQ password, supplied through configuration or a secret provider.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Queue-owned SQL Server database settings.</summary>
public sealed class QueueDatabaseOptions
{
    /// <summary>SQL Server connection string.</summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}

/// <summary>Workflow concurrency, bounded retry, and task timeout values.</summary>
public sealed class QueueProcessingOptions
{
    /// <summary>Maximum number of retry attempts after the initial execution.</summary>
    [Range(0, 100)]
    public int MaxRetryCount { get; set; } = 5;

    /// <summary>Initial retry delay in seconds.</summary>
    [Range(1, 86400)]
    public int InitialRetryDelaySeconds { get; set; } = 5;

    /// <summary>Upper bound on exponential retry delay in seconds.</summary>
    [Range(1, 604800)]
    public int MaxRetryDelaySeconds { get; set; } = 300;

    /// <summary>Maximum time a registered task handler is allowed to run, in seconds.</summary>
    [Range(1, 86400)]
    public int RequestTimeoutSeconds { get; set; } = 60;

    /// <summary>Maximum number of queue task messages processed concurrently by this host.</summary>
    [Range(1, 1000)]
    public int ConcurrentWorkflows { get; set; } = 4;
}
