using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Silo.Domains.Services;
using Xunit;
using Silo.Sync.Core.Checkpoints;
using Silo.Sync.Core.Configuration;
using Silo.Sync.Core.Encryption;
using Silo.Sync.Core.Failures;
using Silo.Sync.Core.Fetching;
using Silo.Sync.Core.Retry;
using Silo.Sync.Core.RunLogging;
using Silo.Sync.Core.Tests.Fakes;
using Silo.Sync.Core.Upsert;
using DbContextOptionsBuilder = Microsoft.EntityFrameworkCore.DbContextOptionsBuilder;

namespace Silo.Sync.Core.Tests.Fixtures;

public sealed class SyncTestDatabaseFixture : IAsyncLifetime
{
    private static readonly string SaPassword = Environment.GetEnvironmentVariable("SILO_SYNC_TEST_SA_PASSWORD") ?? "YourStrong@Passw0rd";
    private string ConnectionString => $"Server=localhost,1433;Database=SiloSyncTests;User Id=SA;Password={SaPassword};TrustServerCertificate=True;";
    private static string StaticMasterConnectionString => $"Server=localhost,1433;Database=master;User Id=SA;Password={SaPassword};TrustServerCertificate=True;";

    public IServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();
        Services = BuildServices();
        await using var scope = Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<WmsApiContext>();
        await context.Database.EnsureCreatedAsync();
        await SeedSourceTableAsync();
        await context.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID('dbo.tbl_SourceProducts', 'U') IS NULL
            CREATE TABLE dbo.tbl_SourceProducts (
                ProductCode NVARCHAR(50) NOT NULL,
                ProductTitle NVARCHAR(250) NULL,
                ProductENTitle NVARCHAR(250) NULL,
                ProductType NVARCHAR(50) NULL,
                ProductTechnicalCode NVARCHAR(50) NULL,
                ProductSize NVARCHAR(50) NULL,
                ProductStatus NVARCHAR(50) NULL,
                ProductUnit NVARCHAR(50) NULL,
                ProductBrand NVARCHAR(128) NULL,
                ProductGroup NVARCHAR(128) NULL,
                ProductSubGroup NVARCHAR(128) NULL,
                ProductClass NVARCHAR(128) NULL,
                ProductPackValue DECIMAL(18,4) NULL,
                ProductValue DECIMAL(18,4) NULL,
                ProductPackWeight DECIMAL(18,4) NULL,
                ProductPackVolume DECIMAL(18,4) NULL,
                ProductCountInPack DECIMAL(18,4) NULL,
                ModifiedAt DATETIME NOT NULL DEFAULT GETDATE()
            );
            """);
    }

    public async Task DisposeAsync()
    {
        await DropDatabaseAsync();
    }

    private static async Task CreateDatabaseAsync()
    {
        await using var connection = new SqlConnection(StaticMasterConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SiloSyncTests') CREATE DATABASE [SiloSyncTests];", connection);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task DropDatabaseAsync()
    {
        try
        {
            await using var connection = new SqlConnection(StaticMasterConnectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("ALTER DATABASE [SiloSyncTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [SiloSyncTests];", connection);
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            // ignored
        }
    }

    private IServiceProvider BuildServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SqlDefaultConnectionString"] = ConnectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddDbContext<WmsApiContext, TestWmsApiContext>(options => options.UseSqlServer(ConnectionString));
        services.AddSingleton<ISyncConnectionStringProtector, PassThroughSyncConnectionStringProtector>();
        services.AddScoped<ISyncSourceConfigProvider, SyncSourceConfigProvider>();
        services.AddScoped<ISyncCheckpointStore, SyncCheckpointStore>();
        services.AddScoped<ISyncRunLogger, SyncRunLogger>();
        services.AddScoped<ISyncFailureStore, SyncFailureStore>();
        services.AddScoped<IRetryScheduler, RetryScheduler>();
        services.AddScoped<ISourceProductFetcher, SourceProductFetcher>();
        services.AddScoped<IProductUpsertService, ProductUpsertService>();
        services.AddScoped<ProductSyncOrchestrator>();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        return services.BuildServiceProvider();
    }

    private async Task SeedSourceTableAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<WmsApiContext>();

        const string createSourceTableSql = """
            IF OBJECT_ID('dbo.tbl_SourceProducts', 'U') IS NOT NULL
            DROP TABLE dbo.tbl_SourceProducts;
            CREATE TABLE dbo.tbl_SourceProducts (
                ProductCode NVARCHAR(50) NULL,
                ProductTitle NVARCHAR(MAX) NULL,
                ProductENTitle NVARCHAR(250) NULL,
                ProductType NVARCHAR(50) NULL,
                ProductTechnicalCode NVARCHAR(50) NULL,
                ProductSize NVARCHAR(50) NULL,
                ProductStatus NVARCHAR(50) NULL,
                ProductUnit NVARCHAR(50) NULL,
                ProductBrand NVARCHAR(128) NULL,
                ProductGroup NVARCHAR(128) NULL,
                ProductSubGroup NVARCHAR(128) NULL,
                ProductClass NVARCHAR(128) NULL,
                ProductPackValue DECIMAL(18,4) NULL,
                ProductValue DECIMAL(18,4) NULL,
                ProductPackWeight DECIMAL(18,4) NULL,
                ProductPackVolume DECIMAL(18,4) NULL,
                ProductCountInPack DECIMAL(18,4) NULL,
                ModifiedAt DATETIME NOT NULL DEFAULT GETDATE()
            );
            """;

        await context.Database.ExecuteSqlRawAsync(createSourceTableSql);
    }

    public async Task ExecuteSourceSqlAsync(string sql)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task InsertSourceProductAsync(string fields, string values)
    {
        await ExecuteSourceSqlAsync($"INSERT INTO dbo.tbl_SourceProducts ({fields}) VALUES ({values});");
    }
}
