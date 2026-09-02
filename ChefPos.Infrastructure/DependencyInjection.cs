using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Settings;
using ChefPos.Infrastructure.Export;
using ChefPos.Infrastructure.Files;
using ChefPos.Infrastructure.Persistence;
using ChefPos.Infrastructure.Repositories;
using ChefPos.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChefPos.Infrastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
   {
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

      services.AddScoped<IOrderRepository, OrderRepository>();
      services.AddScoped<IProductRepository, ProductRepository>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<ICategoryRepository, CategoryRepository>();
      services.AddScoped<ILocationRepository, LocationRepository>();
      services.AddScoped<IIngredientRepository, IngredientRepository>();
      services.AddScoped<IStockRequestRepository,StockRequestRepository>();
      services.AddScoped<ITableRepository, TableRepository>();
      services.AddScoped<IStockMovementRepository, StockMovementRepository>();
      services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
      services.AddScoped<IMenuRepository, MenuRepository>();
      
      services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(); // bu interface ve repository içerisinde IConfiguration servisi kullanıyor ve o servis de Singleton olduğu için JwtTokenGenerator Singleton
      services.Configure<JwtSettings>(configuration.GetSection("Jwt")); // Options pattern ile conf dosyasındaki jwt değerleri = JwtSettings class 
      services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>(); // içerisinde bağımlılık olarak sadece jwtSetting (Options Pattern) içerdiği için Singleton
      services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>(); // içerisinde scoped bağımlılık içermediği için performans için singleton
      services.AddSingleton<IInitialPasswordGenerator, InitialPasswordGenerator>(); // içerisinde scoped bağımlılık içermediği için performans için singleton

      services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName)); // options attern ile conf dosyasındaki fileStorage değerleri = FileStorageSettings class 
      services.AddSingleton<IFileStorageService, LocalFileStorageService>(); // bu interface ve servis içerisinde bağımlılık olarak FileStorageSetting içeriyor o da Singleton.
      services.AddSingleton<IExcelExportService, ExcelExportService>(); // bu interface ve servis içerisinde herhangi bir bağımlılık içermiyor performans ve gereksiz allocate'i önlemek için singleton

      return services;
   } 
}