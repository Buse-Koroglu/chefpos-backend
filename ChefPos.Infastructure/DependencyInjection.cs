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
      
      return services;
   } 
}