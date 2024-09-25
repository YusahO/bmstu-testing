using MewingPad.Common.Entities;
using MewingPad.Common.Enums;
using MewingPad.Database.Context.Roles;
using MewingPad.Services.OAuthService;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Providers;

public static class DatabaseStartupProvider
{
    public static async Task<WebApplication> MigrateDatabaseAsync(
        this WebApplication app
    )
    {
        var context = app.Services.GetRequiredService<AdminDbContext>();
        await context.Database.MigrateAsync();

        return app;
    }

    public static async Task<WebApplication> CreateAdministrator(
        this WebApplication app
    )
    {
        Console.WriteLine("Heelo");
        using var serviceScope = app.Services.CreateScope();
        var authService =
            serviceScope.ServiceProvider.GetRequiredService<IOAuthService>();

        var admin = new User(
            Guid.NewGuid(),
            "admin",
            "admin@mp.ru",
            "admin",
            UserRole.Admin
        );
        await authService.RegisterUser(admin);
        return app;
    }
}
