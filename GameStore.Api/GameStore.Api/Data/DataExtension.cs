using GameStore.Api.Models;
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

    public static void AddGameStoreDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("GameStoreConnection");
        builder.Services.AddSqlite<GameStoreContext>(
            connectionString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Fighthing" },
                        new Genre { Name = "RPG" },
                        new Genre { Name = "Platformer" },
                        new Genre { Name = "Racing" },
                        new Genre { Name = "Sports" }
                    );

                    context.SaveChanges();
                }
            })
        );
    }
}
