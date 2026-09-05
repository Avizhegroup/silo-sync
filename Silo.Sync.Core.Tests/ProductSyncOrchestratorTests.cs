using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silo.Domains.Entities;
using Silo.Domains.Services;
using Silo.Sync.Core.Tests.Fixtures;
using Xunit;

namespace Silo.Sync.Core.Tests;

[Collection("SyncDatabaseCollection")]
public sealed class ProductSyncOrchestratorTests : IClassFixture<SyncTestDatabaseFixture>, IDisposable
{
    private readonly SyncTestDatabaseFixture _fixture;
    private readonly IServiceScope _scope;

    public ProductSyncOrchestratorTests(SyncTestDatabaseFixture fixture)
    {
        _fixture = fixture;
        _scope = fixture.Services.CreateScope();
    }

    private Task InitializeAsync() => Task.CompletedTask;

    public void Dispose()
    {
        _scope.Dispose();
    }

    private ProductSyncOrchestrator Orchestrator => _scope.ServiceProvider.GetRequiredService<ProductSyncOrchestrator>();
    private WmsApiContext Context => _scope.ServiceProvider.GetRequiredService<WmsApiContext>();

    [Fact]
    public async Task Upsert_Product_With_Apostrophe_In_Title_Succeeds()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        await _fixture.ExecuteSourceSqlAsync("""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'PROD-001', N'Product with O''Brien title', N'Type1', N'QC1', N'Size1', N'Unit1', GETDATE());
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.True(result.Success);
        Assert.Equal(1, result.RowsSucceeded);
        var product = await Context.Products.FirstOrDefaultAsync(p => p.Code == "PROD-001");
        Assert.NotNull(product);
        Assert.Contains("O'Brien", product.Title);
    }

    [Fact]
    public async Task Oversized_String_Is_Caught_As_Row_Failure_Not_Unhandled_Exception()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        var longTitle = new string('X', 300);
        await _fixture.ExecuteSourceSqlAsync($"""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'PROD-LONG', N'{longTitle}', N'Type1', N'QC1', N'Size1', N'Unit1', GETDATE());
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.False(result.Success);
        Assert.Equal(0, result.RowsSucceeded);
        Assert.Equal(1, result.RowsFailed);
        var failure = await Context.SyncRowFailures.FirstOrDefaultAsync(f => f.RowKey == "PROD-LONG");
        Assert.NotNull(failure);
        Assert.Equal("Pending", failure.Status);
        Assert.False(string.IsNullOrWhiteSpace(failure.ErrorCategory));
    }

    [Fact]
    public async Task Null_Blank_Key_Is_Skipped_And_Logged()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        await _fixture.ExecuteSourceSqlAsync("""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'', N'Blank Key', N'Type1', N'QC1', N'Size1', N'Unit1', GETDATE()),
                   (NULL, N'Null Key', N'Type1', N'QC1', N'Size1', N'Unit1', GETDATE());
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.True(result.Success);
        Assert.Equal(0, result.RowsSucceeded);
        var runLog = await Context.SyncRunLogs.OrderByDescending(r => r.Id).FirstAsync();
        Assert.Contains("Skipped", runLog.ErrorSummary);
    }

    [Fact]
    public async Task Duplicate_Key_In_Batch_Keeps_First_And_Logs_Second()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        await _fixture.ExecuteSourceSqlAsync("""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'DUP-001', N'First', N'Type1', N'QC1', N'Size1', N'Unit1', DATEADD(MINUTE, 2, GETDATE())),
                   (N'DUP-001', N'Second', N'Type1', N'QC1', N'Size1', N'Unit1', DATEADD(MINUTE, 1, GETDATE()));
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.True(result.Success);
        Assert.Equal(1, result.RowsSucceeded);
        var product = await Context.Products.FirstOrDefaultAsync(p => p.Code == "DUP-001");
        Assert.Equal("First", product?.Title);
        var runLog = await Context.SyncRunLogs.OrderByDescending(r => r.Id).FirstAsync();
        Assert.Contains("Duplicate", runLog.ErrorSummary);
    }

    [Fact]
    public async Task Checkpoint_Advances_To_Max_Succeeded_Value_Not_WallClock()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        var t1 = DateTime.UtcNow.AddHours(-1);
        var t2 = DateTime.UtcNow.AddHours(-2);
        await _fixture.ExecuteSourceSqlAsync($"""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'CHK-001', N'Older', N'Type1', N'QC1', N'Size1', N'Unit1', '{t2:yyyy-MM-dd HH:mm:ss}'),
                   (N'CHK-002', N'Newer', N'Type1', N'QC1', N'Size1', N'Unit1', '{t1:yyyy-MM-dd HH:mm:ss}');
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.True(result.Success);
        Assert.Equal(2, result.RowsSucceeded);
        var checkpoint = await Context.SyncCheckpoints.FirstOrDefaultAsync(c => c.SourceKey == "TEST");
        Assert.NotNull(checkpoint);
        Assert.True(checkpoint.LastCheckpointValue >= t1.AddMinutes(-1));
    }

    [Fact]
    public async Task Failing_Batch_Does_Not_Advance_Checkpoint()
    {
        await ResetAsync();
        await ConfigureSourceAsync("SELECT ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt FROM dbo.tbl_SourceProducts");
        var longTitle = new string('X', 300);
        await _fixture.ExecuteSourceSqlAsync($"""
            INSERT INTO dbo.tbl_SourceProducts (ProductCode, ProductTitle, ProductType, ProductStatus, ProductSize, ProductUnit, ModifiedAt)
            VALUES (N'FAIL-001', N'{longTitle}', N'Type1', N'QC1', N'Size1', N'Unit1', GETDATE());
            """);

        var result = await Orchestrator.RunAsync("TEST");

        Assert.False(result.Success);
        var checkpoint = await Context.SyncCheckpoints.FirstOrDefaultAsync(c => c.SourceKey == "TEST");
        Assert.Null(checkpoint);
    }

    private async Task ConfigureSourceAsync(string command)
    {
        var connectionString = _fixture.Services.GetRequiredService<IConfiguration>()
            .GetConnectionString("SqlDefaultConnectionString")!;

        Context.SyncSourceConfigs.Add(new SyncSourceConfig
        {
            SourceKey = "TEST",
            DisplayName = "Test Source",
            ConnectionStringEncrypted = connectionString,
            Command = command,
            FieldKey = "ProductCode",
            FieldCheck = "ModifiedAt",
            FieldOrder = "ModifiedAt",
            IntervalSeconds = 60,
            IsEnabled = true,
            CreatedDate = DateTime.UtcNow
        });

        await Context.SaveChangesAsync();
    }

    private async Task ResetAsync()
    {
        await _fixture.ExecuteSourceSqlAsync("DELETE FROM dbo.tbl_SourceProducts;");
        Context.SyncSourceConfigs.RemoveRange(Context.SyncSourceConfigs.Where(s => s.SourceKey == "TEST"));
        Context.SyncCheckpoints.RemoveRange(Context.SyncCheckpoints.Where(c => c.SourceKey == "TEST"));
        Context.SyncRunLogs.RemoveRange(Context.SyncRunLogs.Where(r => r.SourceKey == "TEST"));
        Context.SyncRowFailures.RemoveRange(Context.SyncRowFailures.Where(f => f.SourceKey == "TEST"));
        Context.Products.RemoveRange(Context.Products.Where(p => p.Code != null && p.Code.StartsWith("PROD-") || p.Code == "DUP-001" || p.Code == "CHK-001" || p.Code == "CHK-002" || p.Code == "FAIL-001"));
        await Context.SaveChangesAsync();
    }
}
