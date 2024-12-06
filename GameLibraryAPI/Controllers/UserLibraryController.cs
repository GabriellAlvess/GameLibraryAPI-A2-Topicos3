using GameLibraryAPI.DTO;
using GameLibraryAPI.Dtos;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
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
        ///  Adiciona um jogo à biblioteca de um usuário.
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpPost("{id}/library/{gameId}")]
        public IActionResult AddGameToLibrary(int userId, int gameId)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var game = _context.Games.SingleOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                return NotFound(new { Message = "Game not found" });
            }

            if (user.Library.Any(g => g.Id == gameId))
            {
                return BadRequest(new { Message = "Game already in library" });
            }

            user.Library.Add(game);
            return Ok(new { Message = "Game added to library" });
        }

        /// <summary>
        ///  Adiciona uma avaliação a um jogo na biblioteca de um usuário.
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpPost("{gameId}/review")]
        public IActionResult AddReview(int userId, int gameId, [FromBody] AddReviewDto reviewDto)
        {
            // Verifica se o usuário existe
            var user = _context.Users.Include(u => u.Library).SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            // Verifica se o jogo existe
            var game = _context.Games.Include(g => g.Reviews).SingleOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                return NotFound(new { Message = "Game not found" });
            }

            // Verifica se o jogo está na biblioteca do usuário
            if (!user.Library.Any(g => g.Id == gameId))
            {
                return BadRequest(new { Message = "Game not in user's library" });
            }

            // Verifica se o usuário já avaliou o jogo
            if (game.Reviews.Any(r => r.User.Id == userId))
            {
                return Conflict(new { Message = "User already reviewed this game" });
            }

            // Cria o review
            var review = new Review
            {
                Id = _context.Reviews.Any() ? _context.Reviews.Max(r => r.Id) + 1 : 1,
                Game = game, // Referência ao objeto do jogo
                User = user, // Referência ao objeto do usuário
                Comment = reviewDto.Comment,
                Rating = reviewDto.Rating
            };

            // Adiciona o review ao jogo e ao contexto geral
            game.Reviews.Add(review);
            _context.Reviews.Add(review);

            // Retorna o review criado
            var response = new ReviewResponseDto
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username
                },
                game = new GameResponseDto
                {
                    Id = game.Id,
                    Title = game.Title
                }
            };

            return Ok(new { Message = "Review added successfully", Review = response });
        }



        //Mostra a biblioteca de um usuario com os jogos que ele adicionou
        [HttpGet("{id}/library")]
        public IActionResult GetLibrary(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var library = user.Library.Select(g => new GameResponseDto
            {
                Id = g.Id,
                Title = g.Title
            }).ToList();

            return Ok(library);
        }


    }
}
