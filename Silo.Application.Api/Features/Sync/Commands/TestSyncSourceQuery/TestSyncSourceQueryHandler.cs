using System.Data;
using Microsoft.Data.SqlClient;
using Silo.Application.Features;
using Silo.Domains.Services;
using Silo.Sync.Core.Configuration;

namespace Silo.Application.Api.Features.Sync;

public class TestSyncSourceQueryHandler(WmsApiContext context, ISyncSourceConfigProvider configProvider)
    : IRequestHandler<TestSyncSourceQueryCommand, TestSyncSourceQueryVm>
{
    public async Task<TestSyncSourceQueryVm> Handle(TestSyncSourceQueryCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SyncSourceConfigs.FindAsync(new object?[] { request.Id }, cancellationToken);
        if (entity is null)
        {
            return new TestSyncSourceQueryVm { Success = false, ErrorMessage = "Source not found." };
        }

        var config = await configProvider.GetBySourceKeyAsync(entity.SourceKey, cancellationToken);
        if (config is null || string.IsNullOrWhiteSpace(config.ConnectionString))
        {
            return new TestSyncSourceQueryVm
            {
                Success = false,
                ErrorMessage = "Source configuration or connection string is missing."
            };
        }

        try
        {
            await using var connection = new SqlConnection(config.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new SqlCommand(config.Command, connection)
            {
                CommandTimeout = 30
            };

            var dataTable = new DataTable();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            dataTable.Load(reader);

            var columns = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var rows = new List<Dictionary<string, string?>>();
            var sampleSize = Math.Min(request.SampleSize, dataTable.Rows.Count);

            for (var i = 0; i < sampleSize; i++)
            {
                var row = dataTable.Rows[i];
                var dict = new Dictionary<string, string?>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    var value = row[column];
                    dict[column.ColumnName] = value is DBNull or null ? null : value.ToString();
                }
                rows.Add(dict);
            }

            return new TestSyncSourceQueryVm
            {
                Success = true,
                Columns = columns,
                Rows = rows
            };
        }
        catch (Exception ex)
        {
            return new TestSyncSourceQueryVm
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
