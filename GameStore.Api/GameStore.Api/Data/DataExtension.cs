using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
                        .GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }
}
