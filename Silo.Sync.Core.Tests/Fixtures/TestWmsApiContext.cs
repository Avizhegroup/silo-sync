using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Silo.Domains.Entities;
using Silo.Domains.Services;

namespace Silo.Sync.Core.Tests.Fixtures;

public sealed class TestWmsApiContext : WmsApiContext
{
    public TestWmsApiContext(IConfiguration configuration, DbContextOptions<TestWmsApiContext> options)
        : base(configuration, new DbContextOptions<WmsApiContext>(options.Extensions.ToDictionary(e => e.GetType(), e => e)))
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var allowedTypes = new HashSet<Type>
        {
            typeof(Product),
            typeof(SyncSourceConfig),
            typeof(SyncCheckpoint),
            typeof(SyncRunLog),
            typeof(SyncRowFailure)
        };

        foreach (var property in typeof(WmsApiContext).GetProperties())
        {
            if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>))
            {
                continue;
            }

            var entityType = property.PropertyType.GetGenericArguments()[0];
            if (!allowedTypes.Contains(entityType))
            {
                var ignoreMethod = typeof(ModelBuilder).GetMethods()
                    .First(m => m.Name == "Ignore" && m.IsGenericMethod && m.GetParameters().Length == 0)
                    .MakeGenericMethod(entityType);
                ignoreMethod.Invoke(modelBuilder, null);
            }
        }

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("tbl_Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).HasColumnName("ProductCode").HasMaxLength(50);
            entity.Property(e => e.Title).HasColumnName("ProductTitle").HasMaxLength(250);
            entity.Property(e => e.ENTitle).HasColumnName("ProductENTitle").HasMaxLength(250);
            entity.Property(e => e.PackValue).HasColumnName("ProductPackValue");
            entity.Property(e => e.PackWeight).HasColumnName("ProductPackWeight");
            entity.Property(e => e.PackVolume).HasColumnName("ProductPackVolume");
            entity.Property(e => e.CountInPack).HasColumnName("ProductCountInPack");
            entity.Property(e => e.ProductValue).HasColumnName("ProductValue");
            entity.Property(e => e.TechnicalCode).HasColumnName("ProductTechnicalCode").HasMaxLength(50);
            entity.Property(e => e.ProductProperties).HasColumnName("ProductProperties");
            entity.Property(e => e.ProductType).HasColumnName("ProductType").HasMaxLength(50);
            entity.Property(e => e.ProductQc).HasColumnName("ProductStatus").HasMaxLength(50);
            entity.Property(e => e.ProductSize).HasColumnName("ProductSize").HasMaxLength(50);
            entity.Property(e => e.ProductUnit).HasColumnName("ProductUnit").HasMaxLength(50);
            entity.Property(e => e.RegUser).HasColumnName("ProductRegUser").HasMaxLength(50);
            entity.Property(e => e.RegDateTime).HasColumnName("ProductRegDateTime");
            entity.Property(e => e.ProductGalleryId).HasColumnName("ProductGalleryId");
            entity.Property(e => e.TechnicalData).HasColumnName("ProductTechnicalData");
            entity.Property(e => e.ProductGroup).HasColumnName("fld_ProductGroup").HasMaxLength(128);
            entity.Property(e => e.ProductBrand).HasColumnName("fld_ProductBrand").HasMaxLength(128);
            entity.Property(e => e.ProductIsActive).HasColumnName("fld_ProductIsActive");
            entity.Property(e => e.ProductSubGroup).HasColumnName("fld_ProductSubGroup").HasMaxLength(128);
            entity.Property(e => e.ProductClass).HasColumnName("fld_ProductClass").HasMaxLength(128);
        });

        modelBuilder.Entity<SyncSourceConfig>(entity =>
        {
            entity.ToTable("tbl_SyncSourceConfig");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("fld_SyncSourceConfigId").UseIdentityColumn();
            entity.Property(e => e.SourceKey).HasColumnName("fld_SourceKey").HasMaxLength(100);
            entity.HasIndex(e => e.SourceKey).IsUnique();
            entity.Property(e => e.DisplayName).HasColumnName("fld_DisplayName").HasMaxLength(200);
            entity.Property(e => e.SourceType).HasColumnName("fld_SourceType").HasMaxLength(50);
            entity.Property(e => e.ConnectionStringEncrypted).HasColumnName("fld_ConnectionStringEncrypted");
            entity.Property(e => e.Command).HasColumnName("fld_Command");
            entity.Property(e => e.FieldKey).HasColumnName("fld_FieldKey").HasMaxLength(100);
            entity.Property(e => e.FieldCheck).HasColumnName("fld_FieldCheck").HasMaxLength(100);
            entity.Property(e => e.FieldOrder).HasColumnName("fld_FieldOrder").HasMaxLength(100);
            entity.Property(e => e.IntervalSeconds).HasColumnName("fld_IntervalSeconds");
            entity.Property(e => e.IsEnabled).HasColumnName("fld_IsEnabled").HasDefaultValue(true);
            entity.Property(e => e.CreatedBy).HasColumnName("fld_CreatedBy").HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnName("fld_CreatedDate");
            entity.Property(e => e.ModifiedBy).HasColumnName("fld_ModifiedBy").HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnName("fld_ModifiedDate");
        });

        modelBuilder.Entity<SyncCheckpoint>(entity =>
        {
            entity.ToTable("tbl_SyncCheckpoint");
            entity.HasKey(e => e.SourceKey);
            entity.Property(e => e.SourceKey).HasColumnName("fld_SourceKey").HasMaxLength(100);
            entity.Property(e => e.LastCheckpointValue).HasColumnName("fld_LastCheckpointValue");
            entity.Property(e => e.UpdatedDate).HasColumnName("fld_UpdatedDate");
        });

        modelBuilder.Entity<SyncRunLog>(entity =>
        {
            entity.ToTable("tbl_SyncRunLog");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("fld_SyncRunLogId").UseIdentityColumn();
            entity.Property(e => e.SourceKey).HasColumnName("fld_SourceKey").HasMaxLength(100);
            entity.Property(e => e.StartedAt).HasColumnName("fld_StartedAt");
            entity.Property(e => e.FinishedAt).HasColumnName("fld_FinishedAt");
            entity.Property(e => e.RowsFetched).HasColumnName("fld_RowsFetched");
            entity.Property(e => e.RowsSucceeded).HasColumnName("fld_RowsSucceeded");
            entity.Property(e => e.RowsFailed).HasColumnName("fld_RowsFailed");
            entity.Property(e => e.Status).HasColumnName("fld_Status").HasMaxLength(30);
            entity.Property(e => e.ErrorSummary).HasColumnName("fld_ErrorSummary");
        });

        modelBuilder.Entity<SyncRowFailure>(entity =>
        {
            entity.ToTable("tbl_SyncRowFailure");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("fld_SyncRowFailureId").UseIdentityColumn();
            entity.Property(e => e.SyncRunLogId).HasColumnName("fld_SyncRunLogId");
            entity.Property(e => e.SourceKey).HasColumnName("fld_SourceKey").HasMaxLength(100);
            entity.Property(e => e.RowKey).HasColumnName("fld_RowKey").HasMaxLength(200);
            entity.Property(e => e.ErrorCategory).HasColumnName("fld_ErrorCategory").HasMaxLength(100);
            entity.Property(e => e.ErrorMessage).HasColumnName("fld_ErrorMessage");
            entity.Property(e => e.RawPayload).HasColumnName("fld_RawPayload");
            entity.Property(e => e.AttemptCount).HasColumnName("fld_AttemptCount").HasDefaultValue(0);
            entity.Property(e => e.LastAttemptAt).HasColumnName("fld_LastAttemptAt");
            entity.Property(e => e.NextAttemptAt).HasColumnName("fld_NextAttemptAt");
            entity.Property(e => e.Status).HasColumnName("fld_Status").HasMaxLength(30);
            entity.Property(e => e.ResolvedDate).HasColumnName("fld_ResolvedDate");
        });
    }
}
