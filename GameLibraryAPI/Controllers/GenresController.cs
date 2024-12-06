using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly GamesLibraryDbContext _context;

        public GenresController(GamesLibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///  Mostra todos os gêneros de jogos cadastrados e que não foram deletados.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     [
        ///         {
        ///             "id": 1,
        ///             "name": "Genero 1",
        ///             "isDeleted": false
        ///         },
        ///         {
        ///             "id": 2,
        ///             "name": "Genero 2",
        ///             "isDeleted": false
        ///         }
        ///     ]
        /// </remarks>
        /// <response code="200">Retorna os generos cadastrados.</response>
        /// <response code="400">Para casos com erro.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var genres = _context.Genres
                .Where(g => !g.IsDeleted)
                .Select(genres => new GenreResponseDto
                {
                    Id = genres.Id,
                    Name = genres.Name
                })
                .ToList();

            return Ok(genres);
        }

        /// <summary>
        ///  Mostra o gênero pelo id informado.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     {
        ///         "id": 1,
        ///         "name": "Adventure",
        ///         "isDeleted": false
        ///     }
        /// 
        /// </remarks>
        ///<response code="200">Retorna o genero cadastrado.</response>
        ///<response code="400">Para casos com erro.</response>
        ///<response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            var genre = _context.Genres
                .Where(g => g.Id == id && !g.IsDeleted)
                .Select(g => new GenreResponseDto
                {
                  Id = g.Id,
                  Name = g.Name,
                  IsDeleted = g.IsDeleted
                })
                .SingleOrDefault();

            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        /// <summary>
        /// Cria um novo gênero.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     POST
        ///     {
        ///         "name": "New Developer"
        ///     }
        ///     
        /// Exemplo de resposta JSON:
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///         "name": "Adventure",
        ///         "isDeleted": false
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Gênero criado com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] CreateUpdateGenreDto genreDto)
        {
            // Cria uma nova instância de Genre
            var genre = new Genre
            {
                Id = 0, // Força o EF a gerar o ID
                Name = genreDto.Name,
                IsDeleted = false
            };

            // Adiciona ao contexto e salva as alterações de forma assíncrona
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Cria o objeto de resposta
            var response = new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsDeleted = genre.IsDeleted
            };

            // Retorna o status 201 Created com o recurso recém-criado
            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, response);
        }


        /// <summary>
        /// Faz a atualização do gênero informado.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     PUT
        ///     {
        ///         "name": "Updated Developer"
        ///     }
        ///     
        ///Exemplo de resposta JSON:
        ///
        ///     {
        ///         "id": 1,
        ///         "name": "New Genre Name",
        ///         "isDeleted": false
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Gênero atualizado com sucesso.</response>
        /// <response code="404">Gênero não encontrado.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateGenreDto genreDto)
        {
            // Busca o gênero pelo ID
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
            {
                return NotFound(new { Message = "Genre not found" });
            }

            // Atualiza o nome do gênero
            genre.Update(genreDto.Name);

            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Cria o objeto de resposta
            var response = new GenreResponseDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsDeleted = genre.IsDeleted
            };

            // Retorna o status 200 OK com o gênero atualizado
            return Ok(response);
        }


        /// <summary>
        ///  Deleta o gênero informado.
        /// </summary>
        /// <remarks>
        /// Este método define o status `isDeleted` da desenvolvedora como `true`.
        /// </remarks>
        /// <response code="204">Gênero deletado com sucesso.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = _context.Genres.SingleOrDefault(g => g.Id == id);
            if (genre == null)
            {
                return NotFound();
            }
            genre.Delete();
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }

}
