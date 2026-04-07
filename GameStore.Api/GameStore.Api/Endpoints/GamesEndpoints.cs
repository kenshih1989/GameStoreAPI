using GameStore.Api.Dtos;

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
        group.MapPost("/", (CreateGameDto createGameDto) =>
        {
            var newGame = new GameDto(
                Id: games.Max(g => g.Id) + 1,
                Name: createGameDto.Name,
                Genre: createGameDto.Genre,
                Price: createGameDto.Price,
                ReleaseDate: createGameDto.ReleaseDate
            );

            games.Add(newGame);
            // return Results.Created($"/games/{newGame.Id}", newGame);
            return Results.CreatedAtRoute(GetGameEndpointname, new { id = newGame.Id }, newGame);
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
