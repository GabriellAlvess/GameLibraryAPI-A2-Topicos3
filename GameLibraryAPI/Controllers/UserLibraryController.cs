using GameLibraryAPI.DTO;
using GameLibraryAPI.Dtos;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
    [Route("api/userlibrary")]
    [ApiController]
    public class UserLibraryController : ControllerBase
    {
        private readonly GamesLibraryDbContext _context;

        public UserLibraryController(GamesLibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um jogo à biblioteca de um usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     POST
        ///     {
        ///         "message": "Game added to library"
        ///     }
        /// </remarks>
        /// <response code="200">Jogo adicionado com sucesso.</response>
        /// <response code="404">Usuário ou jogo não encontrado.</response>
        /// <response code="400">Jogo já está na biblioteca do usuário.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPost("{userId}/library/{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddGameToLibrary(int userId, int gameId)
        {
            var user = await _context.Users.Include(u => u.Library).SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);
            if (game == null)
            {
                return NotFound(new { Message = "Game not found" });
            }

            if (user.Library.Any(g => g.Id == gameId))
            {
                return BadRequest(new { Message = "Game already in library" });
            }

            user.Library.Add(game);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Game added to library" });
        }

        /// <summary>
        /// Adiciona uma avaliação a um jogo na biblioteca de um usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     POST
        ///     {
        ///         "message": "Review added successfully",
        ///         "review": {
        ///             "id": 1,
        ///             "comment": "Great game!",
        ///             "rating": 5,
        ///             "user": {
        ///                 "id": 1,
        ///                 "username": "JohnDoe"
        ///             },
        ///             "game": {
        ///                 "id": 1,
        ///                 "title": "The Legend of Zelda"
        ///             }
        ///         }
        ///     }
        /// </remarks>
        /// <response code="200">Avaliação adicionada com sucesso.</response>
        /// <response code="404">Usuário ou jogo não encontrado.</response>
        /// <response code="400">Jogo não está na biblioteca do usuário.</response>
        /// <response code="409">Usuário já avaliou este jogo.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPost("{gameId}/review")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddReview(int userId, int gameId, [FromBody] AddReviewDto reviewDto)
        {
            // Verifica se o usuário existe e carrega sua biblioteca
            var user = await _context.Users
                .Include(u => u.Library) // Inclui a biblioteca do usuário
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Verifica se o jogo existe e carrega as avaliações e gêneros
            var game = await _context.Games
                .Include(g => g.Reviews) // Inclui as avaliações do jogo
                .ThenInclude(r => r.User) // Inclui os usuários que fizeram as avaliações
                .Include(g => g.Genres) // Inclui os gêneros do jogo
                .Include(g => g.Developer) // Inclui o desenvolvedor do jogo
                .SingleOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return NotFound(new { Message = "Game not found" });
            }

            // Verifica se o jogo está na biblioteca do usuário
            if (!user.Library?.Any(g => g.Id == gameId) ?? true)
            {
                return BadRequest(new { Message = "Game not in user's library" });
            }

            // Verifica se o usuário já avaliou o jogo
            if (game.Reviews?.Any(r => r.User != null && r.User.Id == userId) ?? false)
            {
                return Conflict(new { Message = "User already reviewed this game" });
            }

            // Cria o review
            var review = new Review
            {
                Game = game,
                User = user,
                Comment = reviewDto.Comment,
                Rating = reviewDto.Rating
            };

            // Adiciona o review ao contexto e salva
            game.Reviews.Add(review);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Retorna o review criado
            var response = new ReviewResponseDto
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                },
                game = new GameResponseDto
                {
                    Id = game.Id,
                    Title = game.Title,
                    Description = game.Description,
                    Developer = new DeveloperResponseDto
                    {
                        Id = game.Developer.Id,
                        Name = game.Developer.Name,
                        IsDeleted = game.Developer.isDeleted
                    },
                    Genres = game.Genres?.Select(genre => new GenreResponseDto
                    {
                        Id = genre.Id,
                        Name = genre.Name,
                        IsDeleted = genre.IsDeleted
                    }).ToList()
                }
            };

            return Ok(new { Message = "Review added successfully", Review = response });
        }


        /// <summary>
        /// Mostra a biblioteca de um usuário com os jogos que ele adicionou.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     [
        ///         {
        ///             "id": 1,
        ///             "title": "The Legend of Zelda"
        ///         }
        ///     ]
        /// </remarks>
        /// <response code="200">Biblioteca retornada com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpGet("{userId}/library")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetLibrary(int userId)
        {
            // Busca o usuário e sua biblioteca
            var user = await _context.Users
                .Include(u => u.Library)
                .ThenInclude(g => g.Developer) 
                .Include(u => u.Library)
                .ThenInclude(g => g.Genres) 
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Mapeia os jogos da biblioteca 
            var library = user.Library.Select(g => new GameResponseDto
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
            }).ToList();

            return Ok(library);
        }

        /// <summary>
        /// Remove um jogo da biblioteca de um usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     DELETE
        ///     {
        ///         "message": "Game removed from library"
        ///     }
        /// </remarks>
        /// <response code="200">Jogo removido com sucesso.</response>
        /// <response code="404">Usuário ou jogo não encontrado.</response>
        /// <response code="400">Jogo não está na biblioteca do usuário.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpDelete("{userId}/library/{gameId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveGameFromLibrary(int userId, int gameId)
        {
            // Busca o usuário e sua biblioteca
            var user = await _context.Users
                .Include(u => u.Library)
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Verifica se o jogo está na biblioteca do usuário
            var game = user.Library.SingleOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                return BadRequest(new { Message = "Game not in user's library" });
            }

            // Remove o jogo da biblioteca
            user.Library.Remove(game);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Game removed from library" });
        }


    }
}
