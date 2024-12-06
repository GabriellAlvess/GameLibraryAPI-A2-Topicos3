using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
    [Route("api/developers")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {
        private readonly GamesLibraryDbContext _context;

        public DevelopersController(GamesLibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mostra a Desenvolvedora com o id especificado.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     {
        ///          "id": 1,
        ///          "name": "String",
        ///          "isDeleted": false
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Retorna os desenvolvedores cadastrados.</response>
        /// <response code="400">Para casos com erro.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAll()
        {
            var developers = _context.Developers
                .Where(d => !d.isDeleted)
                .Select(d => new DeveloperResponseDto
            {
                Id = d.Id,
                Name = d.Name
            })
                .ToList();

            return Ok(developers);
        }

        /// <summary>
        ///  Mostra o desenvolvedor com o id informado.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     {
        ///          "id": 1,
        ///          "name": "String",
        ///          "isDeleted": false
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Retorna o desenvolvedor cadastrado.</response>
        /// <response code="400">Para casos com erro.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetById(int id)
        {
            var developer = _context.Developers
                .Where(d => d.Id == id && !d.isDeleted)
                .Select(d => new DeveloperResponseDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    IsDeleted = d.isDeleted
                })
                .SingleOrDefault();

            if (developer == null)
            {
                return NotFound();
            }
            return Ok(developer);
        }

        /// <summary>
        ///  Cria um novo desenvolvedor.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     POST
        ///     {
        ///          "id": 1,
        ///          "name": "String",
        ///          "isDeleted": false
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Criaçao de desenvolvedor feita com sucesso.</response>
        /// <response code="400">Para casos com erro.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateUpdateDeveloperDto developerDto)
        {
            // Criar uma nova instância de Developer
            var developer = new Developer
            {
                Id = 0, // Força o EF a gerar o ID
                Name = developerDto.Name,
                isDeleted = false
            };

            // Adiciona ao contexto e salva as alterações de forma assíncrona
            _context.Developers.Add(developer);
            await _context.SaveChangesAsync();

            // Cria o objeto de resposta
            var response = new DeveloperResponseDto
            {
                Id = developer.Id,
                Name = developer.Name,
                IsDeleted = developer.isDeleted
            };

            // Retorna o status 201 Created com o recurso recém-criado
            return CreatedAtAction(nameof(GetById), new { id = developer.Id }, response);
        }


        /// <summary>
        ///  Faz a atualização do desenvolvedor com o id informado.
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDeveloperDto developerDto)
        {
            // Busca o desenvolvedor pelo ID
            var developer = await _context.Developers.SingleOrDefaultAsync(d => d.Id == id);
            if (developer == null)
            {
                return NotFound(new { Message = "Developer not found" });
            }

            // Atualiza o nome do desenvolvedor
            developer.Update(developerDto.Name);

            // Salva as alterações de forma assíncrona
            await _context.SaveChangesAsync();

            // Cria o objeto de resposta
            var response = new DeveloperResponseDto
            {
                Id = developer.Id,
                Name = developer.Name,
                IsDeleted = developer.isDeleted
            };

            // Retorna o status 200 OK com o desenvolvedor atualizado
            return Ok(response);
        }

        /// <summary>
        ///  Deleta o desenvolvedor com o id informado.
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var developer = _context.Developers.SingleOrDefault(d => d.Id == id);
            if (developer == null)
            {
                return NotFound();
            }
            developer.Delete();
            return NoContent();
        }
    }
}
