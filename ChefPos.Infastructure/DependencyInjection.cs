using ChefPos.Application.Common.Interfaces;
using ChefPos.Infastructure.Persistence;
using ChefPos.Infastructure.Repositories;
using ChefPos.Infastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChefPos.Infastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddInfastructure(this IServiceCollection services, IConfiguration configuration)
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
      services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
      services.AddScoped<IStockMovementRepository, StockMovementRepository>();
      services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
      services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
      services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
      services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
      services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
      services.AddSingleton<IInitialPasswordGenerator, InitialPasswordGenerator>();
      return services;
   } 
}