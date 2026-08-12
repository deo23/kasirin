namespace KasirIn.Infrastructure;

using KasirIn.Application.Common.Interfaces;
using KasirIn.Infrastructure.Persistence;
using KasirIn.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<KasirInDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        else
        {
            services.AddDbContext<KasirInDbContext>(options =>
                options.UseInMemoryDatabase("KasirInDb"));
        }

        services.AddScoped<IKasirInDbContext>(provider => provider.GetRequiredService<KasirInDbContext>());

        // Storage Services
        services.AddScoped<LocalFileStorageService>();
        services.AddScoped<CloudinaryStorageService>();

        var useCloudinary = configuration.GetValue<bool>("Cloudinary:Enabled");
        if (useCloudinary)
        {
            services.AddScoped<IFileStorageService, CloudinaryStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
        }

        return services;
    }
}
