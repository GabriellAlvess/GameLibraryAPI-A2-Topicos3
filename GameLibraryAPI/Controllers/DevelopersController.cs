using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    /// Mostra todas as desenvolvedoras cadastradas que não foram deletadas.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta JSON:
    /// 
    ///     GET
    ///     [
    ///         {
    ///             "id": 1,
    ///             "name": "Nintendo",
    ///             "isDeleted": false
    ///         },
    ///         {
    ///             "id": 2,
    ///             "name": "Ubisoft",
    ///             "isDeleted": false
    ///         }
    ///     ]
    /// </remarks>
    /// <response code="200">Retorna os Desenvolvedores cadastrados.</response>
    /// <response code="400">Para casos com erro.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var developers = _context.Developers
            .Where(d => !d.isDeleted)
            .Select(d => new DeveloperResponseDto
            {
                Id = d.Id,
                Name = d.Name,
                IsDeleted = d.isDeleted
            })
            .ToList();

        return Ok(developers);
    }

    /// <summary>
    /// Mostra uma desenvolvedora pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Exemplo de resposta JSON:
    /// 
    ///     GET
    ///     {
    ///         "id": 1,
    ///         "name": "Nintendo",
    ///         "isDeleted": false
    ///     }
    /// </remarks>
    ///<response code="200">Retorna o desenvolvedora cadastrado.</response>
    ///<response code="400">Para casos com erro.</response>
    ///<response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id)
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
    /// Cria uma nova desenvolvedora.
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
    ///     {
    ///         "id": 3,
    ///         "name": "New Developer",
    ///         "isDeleted": false
    ///     }
    /// </remarks>
    /// <response code="201">Gênero criado com sucesso.</response>
    /// <response code="400">Requisição inválida.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] CreateUpdateDeveloperDto developerDto)
    {
        var developer = new Developer
        {
            Id = 0, // Força o EF a gerar o ID
            Name = developerDto.Name,
            isDeleted = false
        };

        _context.Developers.Add(developer);
        await _context.SaveChangesAsync();

        var response = new DeveloperResponseDto
        {
            Id = developer.Id,
            Name = developer.Name,
            IsDeleted = developer.isDeleted
        };

        return CreatedAtAction(nameof(GetById), new { id = developer.Id }, response);
    }

    /// <summary>
    /// Atualiza uma desenvolvedora pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Exemplo de corpo de requisição:
    /// 
    ///     PUT
    ///     {
    ///         "name": "Updated Developer"
    ///     }
    /// Exemplo de resposta JSON:
    /// 
    ///     {
    ///         "id": 1,
    ///         "name": "Updated Developer",
    ///         "isDeleted": false
    ///     }
    /// </remarks>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDeveloperDto developerDto)
    {
        var developer = await _context.Developers.SingleOrDefaultAsync(d => d.Id == id);
        if (developer == null)
        {
            return NotFound(new { Message = "Developer not found" });
        }

        developer.Update(developerDto.Name);

        await _context.SaveChangesAsync();

        var response = new DeveloperResponseDto
        {
            Id = developer.Id,
            Name = developer.Name,
            IsDeleted = developer.isDeleted
        };

        return Ok(response);
    }

    /// <summary>
    /// Deleta uma desenvolvedora pelo ID especificado.
    /// </summary>
    /// <remarks>
    /// Este método define o status `isDeleted` da desenvolvedora como `true`.
    /// </remarks>
    /// <response code="204">Gênero deletado com sucesso.</response>
    /// <response code="404">Usuário não encontrado.</response>
    /// <response code="401">Não autorizado, faça login.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var developer = await _context.Developers.SingleOrDefaultAsync(d => d.Id == id);
        if (developer == null)
        {
            return NotFound();
        }

        developer.Delete();
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
