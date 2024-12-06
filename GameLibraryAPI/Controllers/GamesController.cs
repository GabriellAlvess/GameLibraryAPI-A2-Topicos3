using GameLibraryAPI.DTO;
using GameLibraryAPI.Dtos;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
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
        /// Mostra todos os jogos cadastrados que não  foram deletados.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpGet]
        public IActionResult GetAll()
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
                    Genres = g.Genres.Select(g => new GenreResponseDto
                    {
                        Id = g.Id,
                        Name = g.Name
                    }).ToList()
                })
                .ToList();

            return Ok(games);
        }

        /// <summary>
        /// Mostra o jogo com o id especificado.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
       
            var game = _context.Games
                .Where (g => g.Id == id && !g.isDeleted)
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
                    Genres = g.Genres.Select(g => new GenreResponseDto
                    {
                        Id = g.Id,
                        Name = g.Name
                    }).ToList()
                })
                .SingleOrDefault();

            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        /// <summary>
        /// Cria o jogo com os dados fornecidos.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpPost]
        public IActionResult Post([FromBody] CreateUpdatGameDto gameDto)
        {
            // Verificar se o desenvolvedor existe
            var developer = _context.Developers.SingleOrDefault(d => d.Id == gameDto.DeveloperId);
            if (developer == null)
            {
                return BadRequest(new { message = "Developer not found" });
            }

            // Verificar se os gêneros existem
            var genres = _context.Genres.Where(g => gameDto.GenreIds.Contains(g.Id)).ToList();
            if (genres.Count != gameDto.GenreIds.Count)
            {
                return BadRequest(new { message = "One or more genres not found" });
            }

            // Criar o jogo
            var game = new Games
            {
                Id = _context.Games.Any() ? _context.Games.Max(g => g.Id) + 1 : 1,
                Title = gameDto.Title,
                Description = gameDto.Description,
                Developer = developer,
                Genres = genres,
                isDeleted = false
            };

            // Adicionar o jogo ao contexto
            _context.Games.Add(game);

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
                Genres = game.Genres.Select(g => new GenreResponseDto
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList()
            };

            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
        }

        /// <summary>
        /// Atualiza o jogo com o id especificado.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpPut("{id}")]
        public IActionResult Update(int id, Games games)
        {
            var game = _context.Games.SingleOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            game.Update(games.Title, games.Description);
            return NoContent();
        }

        /// <summary>
        /// Deleta o jogo com o id especificado.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            var game = _context.Games.SingleOrDefault(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            game.Delete();
            return NoContent();
        }

        /// <summary>
        /// Mostra o jogo com o id especificado e suas avaliações.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [HttpGet("details/{id}")]
        public IActionResult GetGameDetails(int id)
        {
            var game = _context.Games
                .Include(g => g.Reviews) // Inclui as avaliações do jogo
                .ThenInclude(r => r.User) // Inclui os usuários que fizeram as avaliações
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
                .SingleOrDefault();

            if (game == null)
            {
                return NotFound(new { Message = "Game not found" });
            }

            return Ok(game);
        }


    }
}
