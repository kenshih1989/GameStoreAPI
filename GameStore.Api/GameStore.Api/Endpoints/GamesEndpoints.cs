using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointname = "GetGameById";
    // private static readonly List<GameDto> games = new List<GameDto>
    // {
    //     new GameDto( 1,"The Legend of Zelda: Breath of the Wild","Action-Adventure",59.99m,new DateOnly(2017, 3, 3)),
    //     new GameDto( 2,"Super Mario Odyssey","Platformer",59.99m,new DateOnly(2017, 10, 27)),
    //     new GameDto( 3,"Red Dead Redemption 2","Action-Adventure",59.99m,new DateOnly(2018, 10, 26)),
    //     new GameDto( 4,"The Witcher 3: Wild Hunt","RPG",39.99m,new DateOnly(2015, 5, 19))
    // };

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        #region Get all games and sort by id
        group.MapGet("/", async (GameStoreContext dbContext)
            => await dbContext.Games
                            .Include(game => game.Genre)
                            .OrderBy(game => game.Id)
                            .Select(game => new GameSummaryDto(
                                game.Id,
                                game.Name,
                                game.Genre!.Name,
                                game.Price,
                                game.ReleaseDate
                            ))
                            .AsNoTracking()
                            .ToListAsync());
        #endregion

        #region Get game by id
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            return game is null ? Results.NotFound() : Results.Ok(new GameDetailsDto(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
                ));
        })
        .WithName(GetGameEndpointname);
        #endregion

        #region Create new game
        group.MapPost("/", async (CreateGameDto createGameDto, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = createGameDto.Name,
                GenreId = createGameDto.GenreId,
                Price = createGameDto.Price,
                ReleaseDate = createGameDto.ReleaseDate
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDetailsDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
                );

            return Results.CreatedAtRoute(GetGameEndpointname, new { id = gameDetailsDto.Id }, gameDetailsDto);
        });
        #endregion

        #region Update existing game
        group.MapPut("/{id}", async (
            int id,
            UpdateGameDto updateGameDto,
            GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updateGameDto.Name;
            existingGame.GenreId = updateGameDto.GenreId;
            existingGame.Price = updateGameDto.Price;
            existingGame.ReleaseDate = updateGameDto.ReleaseDate;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });
        #endregion

        #region Delete game by id
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                .Where(game => game.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });
        #endregion

    }
}
