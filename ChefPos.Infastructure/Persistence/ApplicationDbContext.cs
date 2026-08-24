using ChefPos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ChefPos.Infastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options){}
    
    public DbSet<User> Users => Set<User>();
    public DbSet<UserLocation> UserLocations => Set<UserLocation>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ProductItem>  ProductItems => Set<ProductItem>();
    
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    
    public DbSet<StockRequest>  StockRequests => Set<StockRequest>();

    public DbSet<Table> Tables => Set<Table>();
    
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
    
    public DbSet<CategoryLocation> CategoryLocations => Set<CategoryLocation>();
    public DbSet<ProductLocation> ProductLocations => Set<ProductLocation>();
    
    public DbSet<IngredientLot> IngredientLots => Set<IngredientLot>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockMovementLotConsumption> StockMovementLotConsumptions => Set<StockMovementLotConsumption>();

    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuProduct> MenuProducts => Set<MenuProduct>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var idProperty = entityType.FindProperty("Id");
            if (idProperty is not null)
            {
                idProperty.ValueGenerated = ValueGenerated.Never;
            }
        }
    }
}