using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Silo.Domains.Entities;
using Silo.Domains.Entities.Api;

namespace Silo.Domains.Services;

public partial class WmsApiContext(IConfiguration configuration
    , DbContextOptions<WmsApiContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserQuickAccess> UserQuickAccesses { get; set; }
    public DbSet<TruckType> TruckTypes { get; set; }
    public DbSet<DynamicField> DynamicFields { get; set; }
    public DbSet<DynamicFieldSection> DynamicFieldSections { get; set; }
    public DbSet<InputFileData> InputFileDatas { get; set; }
    public DbSet<Gallery> GalleryMedias { get; set; }
    public DbSet<DocumentHeader> DocumentHeaders { get; set; }
    public DbSet<DocumentItem> DocumentItems { get; set; }
    public DbSet<ProductGroup> ProductGroups { get; set; }
    public DbSet<ProductBrand> ProductBrands { get; set; }
    public DbSet<ProductClass> ProductClasses { get; set; }
    public DbSet<ProductSubGroup> ProductSubGroups { get; set; }
    public DbSet<ProductQc> ProductQcs { get; set; }
    public DbSet<Corridor> Corridors { get; set; }
    public DbSet<WarehouseCorridor> WarehouseCorridors { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<FreezeHeader> FreezeHeaders { get; set; }
    public DbSet<FreezeItem> FreezeItems { get; set; }
    public DbSet<NotificationQueue> NotificationQueues { get; set; }
    public DbSet<NotificationEventType> NotificationEventTypes { get; set; }
    public DbSet<NotificationOrder> NotificationOrders { get; set; }
    public DbSet<TruckCrossData> Crosses { get; set; }
    public DbSet<TruckCrossCause> TruckCrossCauses { get; set; }
    public DbSet<TruckCrossCompany> TruckCompanies { get; set; }
    public DbSet<TruckCrossOperationType> TruckCrossOperationTypes { get; set; }
    public DbSet<TruckCrossOperationDestination> TruckCrossOperationDestinations { get; set; }
    public DbSet<TruckCrossShipment> TruckCrossShipments { get; set; }
    public DbSet<TruckCrossCustomer> TruckCrossCustomers { get; set; }
    public DbSet<Entities.TruckCrossProductType> TruckCrossProductTypes { get; set; }
    public DbSet<TruckCrossAcceptPlace> TruckCrossAcceptPlaces { get; set; }
    public DbSet<TruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
    public DbSet<TruckCrossItem> TruckCrossItems { get; set; }
    public DbSet<ActionType> ActionTypes { get; set; }
    public DbSet<WeighBridgeLog> WeighbridgeLogs { get; set; }
    public DbSet<ReportFormat> ReportFormats { get; set; }
    public DbSet<MenuLink> MenuLinks { get; set; }
    public DbSet<DocumentLog> DocumentLogs { get; set; }
    public DbSet<DocumentStatus> DocumentStatuses { get; set; }
    public DbSet<ExpireGuaranteeLog> ExpireGuaranteeLogs { get; set; }
    public DbSet<NonDocFileLog> NonDocFileLogs { get; set; }
    public DbSet<CustomerAccountingData> CustomerAccountingDatas { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Province> Provinces { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<SalesShop> SalesShops { get; set; }
    public DbSet<SalesInstaller> SalesInstallers { get; set; }
    public DbSet<MovementAction> MovementActions { get; set; }
    public DbSet<TagsMovement> TagsMovements { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<UHFReaderLogHeader> UHFReaderLogHeaders { get; set; }
    public DbSet<UHFReaderLogItem> UHFReaderLogItems { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Line> Lines { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ActionTypeControls> ActionTypeControls { get; set; }
    public DbSet<WarehouseType> WarehouseTypes { get; set; }
    public DbSet<Items> Items { get; set; }
    public DbSet<PreparedReport> PreparedReports { get; set; }
    public DbSet<Print> Prints { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<ChatSessions> ChatSessions { get; set; }
    public DbSet<PrintFormat> PrintFormats { get; set; }
    public DbSet<TablesChangeLog> TagChangeLog { get; set; }
    public DbSet<GPSLogs> GpsLogs { get; set; }
    public DbSet<TextResource> TextResources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TablesChangeLog>(entity =>
        {
            entity.Property(x => x.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            entity.HasOne(p => p.User)
                  .WithMany(p => p.ChangeLogs)
                  .HasForeignKey(p => p.UserId);
        });

        modelBuilder.Entity<TextResource>(entity =>
        {
            entity.HasIndex(e => e.Key)
                  .IsUnique();
        });

        modelBuilder.Entity<ChatSessions>(entity =>
        {
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("GETDATE()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TokenUsage)
                .HasColumnType("json");

            entity.Property(e => e.PriceUsage)
               .HasColumnType("decimal(18,8)")
               .HasDefaultValue(0m);
        });

        modelBuilder.Entity<Entities.TruckCrossData>()
                    .HasOne(p => p.PresentUser)
                    .WithMany(p => p.PresentCrosses)
                    .HasForeignKey(p => p.PresentUserId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                    .HasOne(p => p.PresentRevokeUser)
                    .WithMany(p => p.PresentRevokeCrosses)
                    .HasForeignKey(p => p.PresentRevokeUserId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                    .HasOne(p => p.EnterUser)
                    .WithMany(p => p.EnterCrosses)
                    .HasForeignKey(p => p.EnterUserId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                    .HasOne(p => p.ExitUser)
                    .WithMany(p => p.ExitCrosses)
                    .HasForeignKey(p => p.ExitUserId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.Type)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.TypeId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.Cause)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentCause);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.TruckCrossCompany)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.TruckCrossCompanyId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.Customer)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentCustomerId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.OperationType)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentOperationTypeId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.OperationDestination)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentOperationDestinationId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.Shipment)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentShipmentId);

        modelBuilder.Entity<Entities.TruckCrossData>()
                   .HasOne(p => p.Customer)
                   .WithMany(p => p.TruckCrosses)
                   .HasForeignKey(p => p.PresentCustomerId);

        modelBuilder.Entity<Gallery>()
                   .HasOne(p => p.User)
                   .WithMany(p => p.GalleryMedias)
                   .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<DynamicField>()
                   .HasOne(p => p.User)
                   .WithMany(p => p.DynamicFields)
                   .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<DocumentItem>()
            .HasOne(p => p.DocumentHeader)
            .WithMany(p => p.DocumentItems)
            .HasForeignKey(p => p.Key)
            .HasPrincipalKey(p => p.Key);

        modelBuilder.Entity<Corridor>()
                    .HasOne(p => p.Warehouse)
                    .WithMany(p => p.Corridors)
                    .HasForeignKey(p => p.WarehouseCode);

        modelBuilder.Entity<Zone>()
                    .HasOne(p => p.Warehouse)
                    .WithMany(p => p.Zones)
                    .HasForeignKey(p => p.WarehouseCode);

        modelBuilder.Entity<Zone>()
                   .HasOne(p => p.Corridor)
                   .WithMany(p => p.Zones)
                   .HasForeignKey(p => p.CorridorId);

        modelBuilder.Entity<Warehouse>()
                    .Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<ProductSize>()
                    .Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        modelBuilder.Entity<FreezeHeader>()
                    .HasOne(p => p.User)
                    .WithMany(p => p.FreezeHeaders)
                    .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<FreezeItem>()
                    .HasOne(p => p.FreezeHeader)
                    .WithMany(p => p.FreezeItems)
                    .HasForeignKey(p => p.FreezeHeaderId);

        modelBuilder.Entity<TruckCrossOperationType>()
                    .HasOne(p => p.TruckCrossCause)
                    .WithMany(p => p.TruckCrossOperationTypes)
                    .HasForeignKey(p => p.TruckCrossCauseId);

        modelBuilder.Entity<TruckCrossItem>()
                    .HasOne(p => p.TruckCross)
                    .WithMany(p => p.TruckCrossItems)
                    .HasForeignKey(p => p.TruckCrossId);

        modelBuilder.Entity<TruckCrossItem>()
                    .HasOne(p => p.TruckCrossProductType)
                    .WithMany(p => p.TruckCrossItems)
                    .HasForeignKey(p => p.TruckCrossProductTypeId);

        modelBuilder.Entity<ReportFormat>()
                    .HasOne(p => p.User)
                    .WithMany(p => p.ReportFormats)
                    .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<DocumentLog>()
           .HasOne(p => p.User)
           .WithMany(p => p.DocumentLogs)
           .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<MenuLink>()
                    .HasOne(p => p.Parent)
                    .WithMany(p => p.ChildrenLinks)
                    .HasForeignKey(p => p.ParentId);

        modelBuilder.Entity<DocumentHeader>()
          .HasOne(p => p.DocumentStatus)
          .WithMany(p => p.DocumentHeaders)
          .HasForeignKey(p => p.DocumentStatusId);

        modelBuilder.Entity<ExpireGuaranteeLog>()
                    .HasOne(p => p.User)
                    .WithMany(p => p.ExpireGuaranteeLogs)
                    .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<DocumentHeader>()
            .HasOne(p => p.User)
            .WithMany(p => p.DocumentHeaders)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<DocumentHeader>()
            .HasOne(p => p.UserStatus)
            .WithMany(p => p.DocumentHeadersStatus)
            .HasForeignKey(p => p.ChangeStatusLastUserId);

        modelBuilder.Entity<CustomerAccountingData>()
            .Property(p => p.ProductCount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ProductSubGroup>()
            .HasOne(p => p.ProductGroup)
            .WithMany(p => p.ProductSubGroups)
            .HasForeignKey(p => p.ProductGroupCode)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<NotificationQueue>()
            .HasOne(p => p.NotificationOrder)
            .WithMany(p => p.NotificationQueues)
            .HasForeignKey(p => p.OrderId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductSizeEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductSize);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductGroupEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductGroup)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductSubGroupEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductSubGroup)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductBrandEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductBrand)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductClassEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductClass)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductTypeEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductType)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductQcEntity)
            .WithMany(p => p.Products)
            .HasForeignKey(p => p.ProductQc)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<City>()
            .HasOne(p => p.Province)
            .WithMany(p => p.Cities)
            .HasForeignKey(p => p.ProvinceId);

        modelBuilder.Entity<SalesShop>()
            .HasOne(p => p.User)
            .WithMany(p => p.SalesShops)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<SalesInstaller>()
            .HasOne(p => p.User)
            .WithMany(p => p.SalesInstallers)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<TruckCrossShipmentFee>()
            .HasOne(p => p.TruckCrossCompany)
            .WithMany(p => p.TruckCrossShipmentFees)
            .HasForeignKey(p => p.CompanyId);

        modelBuilder.Entity<TruckCrossShipmentFee>()
            .HasOne(p => p.TruckCrossCustomer)
            .WithMany(p => p.TruckCrossShipmentFees)
            .HasForeignKey(p => p.CustomerId);

        modelBuilder.Entity<TruckCrossShipmentFee>()
            .HasOne(p => p.TruckCrossProductType)
            .WithMany(p => p.TruckCrossShipmentFees)
            .HasForeignKey(p => p.ProductTypeId);

        modelBuilder.Entity<TruckCrossShipmentFee>()
            .HasOne(p => p.TruckCrossShipment)
            .WithMany(p => p.TruckCrossShipmentFees)
            .HasForeignKey(p => p.ShipmentId);

        modelBuilder.Entity<Shift>()
           .HasOne(p => p.Line)
           .WithMany(p => p.Shifts)
           .HasForeignKey(p => p.LineId);

        modelBuilder.Entity<UHFReaderLogHeader>()
            .HasOne(p => p.Station)
            .WithMany(p => p.UHFReaderLogHeaders)
            .HasForeignKey(p => p.StationCode)
            .HasPrincipalKey(p => p.Code);

        modelBuilder.Entity<UHFReaderLogHeader>()
            .HasOne(p => p.DocumentHeader)
            .WithMany(p => p.UHFReaderLogHeaders)
            .HasForeignKey(p => p.DocumentCode)
            .HasPrincipalKey(p => p.Key);

        modelBuilder.Entity<UHFReaderLogHeader>()
            .HasOne(p => p.TruckCross)
            .WithMany(p => p.UHFReaderLogHeaders)
            .HasForeignKey(p => p.TruckCrossId);

        modelBuilder.Entity<UHFReaderLogHeader>()
            .HasOne(p => p.User)
            .WithMany(p => p.UHFReaderLogHeaders)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<UHFReaderLogHeader>()
            .HasOne(p => p.MovementAction)
            .WithMany(p => p.UHFReaderLogHeaders)
            .HasForeignKey(p => p.MovementActionId);

        modelBuilder.Entity<UHFReaderLogItem>()
            .HasOne(p => p.UHFReaderLogHeader)
            .WithMany(p => p.UHFReaderLogItems)
            .HasForeignKey(p => p.UHFReaderLogHeaderId);

        modelBuilder.Entity<UserClaim>()
            .HasOne(p => p.User)
            .WithMany(p => p.UserClaims)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<TagsMovement>()
            .HasOne(p => p.MovementAction)
            .WithMany(p => p.TagsMovements)
            .HasForeignKey(p => p.RMovementActionId);

        modelBuilder.Entity<TagsMovement>()
            .HasOne(p => p.MovementAction)
            .WithMany(p => p.TagsMovements)
            .HasForeignKey(p => p.RMovementActionId);

        modelBuilder.Entity<TagsMovement>()
            .HasOne(tm => tm.Tag)
            .WithMany(t => t.TagsMovements)
            .HasForeignKey(tm => tm.ProductSerial)
            .HasPrincipalKey(t => t.ProductSerial)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TagsMovement>()
            .HasOne(tm => tm.Product)
            .WithMany(t => t.TagsMovements)
            .HasForeignKey(tm => tm.ProductCode)
            .HasPrincipalKey(t => t.Code)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TruckCrossCause>()
           .HasOne(p => p.EnterActionType)
           .WithMany(p => p.EnterTruckCrossCauses)
           .HasForeignKey(p => p.EnterActionTypeId);

        modelBuilder.Entity<TruckCrossCause>()
          .HasOne(p => p.ExitActionType)
          .WithMany(p => p.ExitTruckCrossCauses)
          .HasForeignKey(p => p.ExitActionTypeId);

        modelBuilder.Entity<DynamicField>()
                    .HasOne(p => p.DynamicFieldSection)
                    .WithMany(p => p.DynamicFields)
                    .HasForeignKey(p => p.SectionId);

        modelBuilder.Entity<PreparedReport>()
              .HasOne(p => p.User)
              .WithMany(p => p.PreparedReports)
              .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<UserToken>()
             .HasOne(p => p.User)
             .WithMany(p => p.Tokens)
             .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Print>()
             .HasOne(p => p.SoftDeleteUser)
             .WithMany(p => p.Prints)
             .HasForeignKey(p => p.SoftDeleteUserId);

        modelBuilder.Entity<GPSLogs>()
           .HasOne(p => p.User)
           .WithMany(p => p.GPSLogs)
           .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<UserQuickAccess>()
            .HasIndex(p => new { p.UserId, p.MenuLinkId })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
