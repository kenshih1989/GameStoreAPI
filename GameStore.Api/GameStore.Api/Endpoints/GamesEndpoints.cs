using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointname = "GetGameById";
    private static readonly List<GameDto> games = new List<GameDto>
    {
        new GameDto( 1,"The Legend of Zelda: Breath of the Wild","Action-Adventure",59.99m,new DateOnly(2017, 3, 3)),
        new GameDto( 2,"Super Mario Odyssey","Platformer",59.99m,new DateOnly(2017, 10, 27)),
        new GameDto( 3,"Red Dead Redemption 2","Action-Adventure",59.99m,new DateOnly(2018, 10, 26)),
        new GameDto( 4,"The Witcher 3: Wild Hunt","RPG",39.99m,new DateOnly(2015, 5, 19))
    };

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");

        #region Get all games and sort by id
        group.MapGet("/", () => games.OrderBy(g => g.Id).ToList());
        #endregion

        #region Get game by id
        group.MapGet("/{id}", (int id) =>
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        })
        .WithName(GetGameEndpointname);
        #endregion

        #region Create new game
        group.MapPost("/", (CreateGameDto createGameDto, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
                Name = createGameDto.Name,
                GenreId = createGameDto.GenreId,
                Price = createGameDto.Price,
                ReleaseDate = createGameDto.ReleaseDate
            };

            dbContext.Games.Add(game);
            dbContext.SaveChanges();

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
        group.MapPut("/{id}", (int id, UpdateGameDto updateGameDto) =>
        {
            var existingGame = games.FirstOrDefault(g => g.Id == id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }

            var updatedGame = existingGame with
            {
                Name = updateGameDto.Name,
                Genre = updateGameDto.Genre,
                Price = updateGameDto.Price,
                ReleaseDate = updateGameDto.ReleaseDate
            };

            games.Remove(existingGame);
            games.Add(updatedGame);

            return Results.Ok(updatedGame);
        });
        #endregion

        #region Delete game by id
        group.MapDelete("/{id}", (int id) =>
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game is null)
            {
                return Results.NotFound();
            }

            games.Remove(game);
            return Results.NoContent();
        });
        #endregion

    }
}
