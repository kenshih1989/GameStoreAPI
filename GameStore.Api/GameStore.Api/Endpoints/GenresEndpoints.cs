using GameStore.Api.Data;
using GameStore.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
  public static void MapGenresEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/genres");

    #region Get all genres and sort by id
    group.MapGet("/", async (GameStoreContext dbContext)
      => await dbContext.Genres
                        .OrderBy(genres => genres.Id)
                        .Select(genres => new GenresDto(
                            genres.Id,
                            genres.Name
                        ))
                        .AsNoTracking()
                        .ToListAsync());
    #endregion
  }
}
