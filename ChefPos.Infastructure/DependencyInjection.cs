using ChefPos.Application.Common.Interfaces;
using ChefPos.Infastructure.Persistence;
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

      services.AddScoped<IOrderRepository, IOrderRepository>();
      services.AddScoped<IProductRepository, IProductRepository>();
      services.AddScoped<IUserRepository, IUserRepository>();
      
      return services;
   } 
}