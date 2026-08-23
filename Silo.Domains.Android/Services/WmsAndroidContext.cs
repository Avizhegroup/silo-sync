using Microsoft.EntityFrameworkCore;
using Silo.Domains.Android.Entities;

namespace Silo.Domains.Android;

public class WmsAndroidContext : DbContext
{
    public WmsAndroidContext(DbContextOptions<WmsAndroidContext> context) : base(context)
    {
    }

    public DbSet<Register> Registers { get; set; }
    public DbSet<SavedAction> SavedActions { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<UnRegister> UnRegisters { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<InspectElement> Elements { get; set; }
    public DbSet<CustomerAccountingData> Cad { get; set; }
    public DbSet<InventoryTags> InventoryTags { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Destination> Destinations { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<AndroidShift> Shifts { get; set; }
    public DbSet<AndroidLine> Lines { get; set; }
    public DbSet<AndroidActionType> ActionTypes { get; set; }
    public DbSet<TextResourceEntity> TextResourceEntities { get; set; }
    public DbSet<AndroidProductType> ProductTypes { get; set; }
    public DbSet<AndroidUser> Users { get; set; }
    public DbSet<AndroidPermission> Permissions { get; set; }
    public DbSet<AndroidProductBrand> ProductBrand { get; set; }
    public DbSet<AndroidProductSize> ProductSize { get; set; }
    public DbSet<AndroidProductSubGroup> ProductSubGroup { get; set; }
    public DbSet<AndroidProductGroup> ProductGroup { get; set; }
    public DbSet<AndroidProductStatus> ProductStatus { get; set; }
    public DbSet<AndroidGallery> GalleryMedias { get; set; }
    public DbSet<AndroidUhfLog> UhfLogs { get; set; }
    public DbSet<AndroidUnsync> Unsyncs { get; set; }
    public DbSet<AndroidItems> Items { get; set; }
    public DbSet<AndroidTruckCrossCause> TruckCrossCauses { get; set; }
    public DbSet<AndroidTruckCrossCompany> TruckCompanies { get; set; }
    public DbSet<AndroidTruckCrossOperationType> TruckCrossOperationTypes { get; set; }
    public DbSet<AndroidTruckCrossOperationDestination> TruckCrossOperationDestinations { get; set; }
    public DbSet<AndroidTruckCrossShipment> TruckCrossShipments { get; set; }
    public DbSet<AndroidTruckCrossCustomer> TruckCrossCustomers { get; set; }
    public DbSet<AndroidTruckCrossProductType> TruckCrossProductTypes { get; set; }
    public DbSet<AndroidTruckCrossAcceptPlace> TruckCrossAcceptPlaces { get; set; }
    public DbSet<AndroidTruckCrossShipmentFee> TruckCrossShipmentFees { get; set; }
    public DbSet<AndroidTruckType> TruckTypes { get; set; }
    public DbSet<AndroidDynamicFields> DynamicFields { get; set; }
    public DbSet<AndroidDynamicFieldSection> DynamicFieldSections { get; set; }
    public DbSet<AndroidDocumentStatus> DocumentStatuses { get; set; }
    public DbSet<AndroidPrint> Prints { get; set; }
    public DbSet<AndroidProductClass> ProductClasses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryTags>(entity =>
        {
            entity.HasIndex(e => new { e.Epc, e.HeaderId }).IsUnique();
        });

        modelBuilder.Entity<AndroidUnsync>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<AndroidItems>(entity =>
        {
            entity.HasKey(p => p.Id);
        });

        base.OnModelCreating(modelBuilder);
    }

    public void DeleteData()
    {
        Registers.ExecuteDelete();

        SavedActions.ExecuteDelete();

        Tags.ExecuteDelete();

        Products.ExecuteDelete();

        Elements.ExecuteDelete();

        UnRegisters.ExecuteDelete();

        Cad.ExecuteDelete();

        InventoryTags.ExecuteDelete();

        Zones.ExecuteDelete();

        DynamicFields.ExecuteDelete();

        Destinations.ExecuteDelete();

        Stations.ExecuteDelete();

        Shifts.ExecuteDelete();

        Lines.ExecuteDelete();

        ActionTypes.ExecuteDelete();

        TextResourceEntities.ExecuteDelete();

        ProductTypes.ExecuteDelete();

        Users.ExecuteDelete();

        Permissions.ExecuteDelete();

        GalleryMedias.ExecuteDelete();

        ProductBrand.ExecuteDelete();

        ProductSize.ExecuteDelete();

        ProductSubGroup.ExecuteDelete();

        ProductGroup.ExecuteDelete();

        ProductStatus.ExecuteDelete();

        UhfLogs.ExecuteDelete();

        Unsyncs.ExecuteDelete();

        Items.ExecuteDelete();

        TruckCrossShipmentFees.ExecuteDelete();

        TruckCrossAcceptPlaces.ExecuteDelete();

        TruckCrossProductTypes.ExecuteDelete();

        TruckCrossCustomers.ExecuteDelete();

        TruckCrossShipments.ExecuteDelete();

        TruckCrossOperationDestinations.ExecuteDelete();

        TruckCrossOperationTypes.ExecuteDelete();

        TruckCompanies.ExecuteDelete();

        TruckCrossCauses.ExecuteDelete();

        DynamicFields.ExecuteDelete();

        DynamicFieldSections.ExecuteDelete();

        TruckTypes.ExecuteDelete();

        DocumentStatuses.ExecuteDelete();

        Prints.ExecuteDelete();

        ProductClasses.ExecuteDelete();

        Database.ExecuteSqlRaw("VACUUM;");
    }
}
