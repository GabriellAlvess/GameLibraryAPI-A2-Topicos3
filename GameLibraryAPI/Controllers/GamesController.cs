using GameLibraryAPI.DTO;
using GameLibraryAPI.Dtos;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/games")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly GamesLibraryDbContext _context;

    public GamesController(GamesLibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Mostra todos os jogos cadastrados que não foram deletados.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta JSON:
    /// 
    ///     GET
    ///     [
    ///         {
    ///             "id": 1,
    ///             "title": "The Legend of Zelda",
    ///             "description": "An epic adventure game.",
    ///             "developer": {
    ///                 "id": 1,
    ///                 "name": "Nintendo"
    ///             },
    ///             "genres": [
    ///                 { "id": 1, "name": "Adventure" },
    ///                 { "id": 2, "name": "Action" }
    ///             ]
    ///         }
    ///     ]
    /// </remarks>
    /// <response code="200">Retorna os jogos cadastrados.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var games = _context.Games
            .Where(g => !g.isDeleted)
            .Select(g => new GameResponseDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Developer = new DeveloperResponseDto
                {
                    Id = g.Developer.Id,
                    Name = g.Developer.Name
                },
                Genres = g.Genres.Select(genre => new GenreResponseDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                }).ToList()
            })
            .ToList();

        return Ok(games);
    }

    /// <summary>
    /// Mostra um jogo pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta JSON:
    /// 
    ///     GET
    ///     {
    ///         "id": 1,
    ///         "title": "The Legend of Zelda",
    ///         "description": "An epic adventure game.",
    ///         "developer": {
    ///             "id": 1,
    ///             "name": "Nintendo"
    ///         },
    ///         "genres": [
    ///             { "id": 1, "name": "Adventure" },
    ///             { "id": 2, "name": "Action" }
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="200">Retorna o jogo solicitado.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
    {
        var game = _context.Games
            .Where(g => g.Id == id && !g.isDeleted)
            .Select(g => new GameResponseDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Developer = new DeveloperResponseDto
                {
                    Id = g.Developer.Id,
                    Name = g.Developer.Name
                },
                Genres = g.Genres.Select(genre => new GenreResponseDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                }).ToList()
            })
            .SingleOrDefault();

        if (game == null)
        {
            return NotFound(new { Message = "Game not found" });
        }

        return Ok(game);
    }

    /// <summary>
    /// Cria um novo jogo.
    /// </summary>
    /// <remarks>
    /// Exemplo de corpo de requisição:
    /// 
    ///     POST
    ///     {
    ///         "title": "New Game",
    ///         "description": "A great game.",
    ///         "developerId": 1,
    ///         "genreIds": [1, 2]
    ///     }
    /// Exemplo de resposta JSON:
    /// 
    ///     {
    ///         "id": 2,
    ///         "title": "New Game",
    ///         "description": "A great game.",
    ///         "developer": {
    ///             "id": 1,
    ///             "name": "Nintendo"
    ///         },
    ///         "genres": [
    ///             { "id": 1, "name": "Adventure" },
    ///             { "id": 2, "name": "Action" }
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="201">Jogo criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] CreateUpdatGameDto gameDto)
    {
        var developer = _context.Developers.SingleOrDefault(d => d.Id == gameDto.DeveloperId);
        if (developer == null)
        {
            return BadRequest(new { message = "Developer not found" });
        }

        var genres = _context.Genres.Where(g => gameDto.GenreIds.Contains(g.Id)).ToList();
        if (genres.Count != gameDto.GenreIds.Count)
        {
            return BadRequest(new { message = "One or more genres not found" });
        }

        var game = new Games
        {
            Title = gameDto.Title,
            Description = gameDto.Description,
            Developer = developer,
            Genres = genres,
            isDeleted = false
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var response = new GameResponseDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Developer = new DeveloperResponseDto
            {
                Id = game.Developer.Id,
                Name = game.Developer.Name
            },
            Genres = game.Genres.Select(genre => new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = game.Id }, response);
    }

    /// <summary>
    /// Atualiza um jogo pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Exemplo de corpo de requisição:
    /// 
    ///     PUT
    ///     {
    ///         "title": "Updated Game",
    ///         "description": "Updated description.",
    ///         "developerId": 1,
    ///         "genreIds": [1, 2]
    ///     }
    /// </remarks>
    /// <response code="200">Jogo atualizado com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdatGameDto gameDto)
    {
        var game = await _context.Games.Include(g => g.Genres).Include(g => g.Developer).SingleOrDefaultAsync(g => g.Id == id);
        if (game == null)
        {
            return NotFound(new { Message = "Game not found" });
        }

        var developer = await _context.Developers.SingleOrDefaultAsync(d => d.Id == gameDto.DeveloperId);
        if (developer == null)
        {
            return BadRequest(new { Message = "Developer not found" });
        }

        var genres = await _context.Genres.Where(g => gameDto.GenreIds.Contains(g.Id)).ToListAsync();
        if (genres.Count != gameDto.GenreIds.Count)
        {
            return BadRequest(new { Message = "One or more genres not found" });
        }

        // Atualiza as propriedades do jogo
        game.Update(gameDto.Title, gameDto.Description, developer, genres);

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Game updated successfully" });
    }


    /// <summary>
    /// Deleta um jogo pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Este método define o status `isDeleted` do jogo como `true`.
    /// </remarks>
    /// <response code="204">Jogo deletado com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var game = _context.Games.SingleOrDefault(g => g.Id == id);
        if (game == null)
        {
            return NotFound(new { Message = "Game not found" });
        }

        game.Delete();
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Mostra o jogo com o ID especificado e suas avaliações.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta JSON:
    /// 
    ///     GET
    ///     {
    ///         "id": 1,
    ///         "title": "The Legend of Zelda",
    ///         "description": "An epic adventure game.",
    ///         "developer": {
    ///             "id": 1,
    ///             "name": "Nintendo"
    ///         },
    ///         "genres": [
    ///             { "id": 1, "name": "Adventure" },
    ///             { "id": 2, "name": "Action" }
    ///         ],
    ///         "averageRating": 4.5,
    ///         "reviews": [
    ///             {
    ///                 "id": 1,
    ///                 "comment": "Great game!",
    ///                 "rating": 5,
    ///                 "user": {
    ///                     "id": 1,
    ///                     "username": "JohnDoe"
    ///                 }
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="200">Retorna os detalhes do jogo.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpGet("details/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGameDetails(int id)
    {
        var game = await _context.Games
            .Include(g => g.Developer)
            .Include(g => g.Genres)
            .Include(g => g.Reviews)
            .ThenInclude(r => r.User)
            .Where(g => g.Id == id && !g.isDeleted)
            .Select(g => new
            {
                g.Id,
                g.Title,
                g.Description,
                Developer = new
                {
                    g.Developer.Id,
                    g.Developer.Name
                },
                Genres = g.Genres.Select(genre => new
                {
                    genre.Id,
                    genre.Name
                }).ToList(),
                AverageRating = g.Reviews.Any() ? g.Reviews.Average(r => r.Rating) : 0.0,
                Reviews = g.Reviews.Select(r => new
                {
                    r.Id,
                    r.Comment,
                    r.Rating,
                    User = new
                    {
                        r.User.Id,
                        r.User.Username
                    }
                }).ToList()
            })
            .SingleOrDefaultAsync();

        if (game == null)
        {
            return NotFound(new { Message = "Game not found" });
        }

        return Ok(game);
    }

}



