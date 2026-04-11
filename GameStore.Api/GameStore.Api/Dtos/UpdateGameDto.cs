using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record UpdateGameDto(
    [Required][StringLength(50)] string Name,
    [Range(1,60)] int GenreId,
    [Range(1,100)]decimal Price,
    [Required]DateOnly ReleaseDate
);
