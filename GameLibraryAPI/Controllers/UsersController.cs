using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GamesLibraryDbContext _context;

        public UsersController(GamesLibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mostra todos os usuários cadastrados que não foram deletados.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     [
        ///         {
        ///             "id": 1,
        ///             "username": "JohnDoe",
        ///             "email": "johndoe@example.com"
        ///         }
        ///     ]
        /// </remarks>
        /// <response code="200">Retorna os usuários cadastrados.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Where(u => !u.isDeleted)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Mostra um usuário pelo ID especificado.
        /// </summary>
        /// <remarks>
        /// Exemplo de resposta JSON:
        /// 
        ///     GET
        ///     {
        ///         "id": 1,
        ///         "username": "JohnDoe",
        ///         "email": "johndoe@example.com"
        ///     }
        /// </remarks>
        /// <response code="200">Retorna o usuário solicitado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id && !u.isDeleted)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     POST
        ///     {
        ///         "username": "JohnDoe",
        ///         "password": "1234",
        ///         "email": "johndoe@example.com"
        ///     }
        ///     
        /// Exemplo de resposta JSON:
        /// 
        ///     {
        ///         "id": 1,
        ///         "username": "JohnDoe",
        ///         "email": "johndoe@example.com"
        ///     }
        /// </remarks>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Requisição inválida.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateUpdateUserDto userDto)
        {
            // Verifica se o email já existe
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return BadRequest(new { Message = "Email already in use" });
            }

            var user = new User
            {
                Id = 0, // Força o EF a gerar o ID
                Username = userDto.Username,
                Password = userDto.Password,
                Email = userDto.Email,
                isDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, response);
        }

        /// <summary>
        /// Atualiza um usuário pelo ID especificado.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     PUT
        ///     {
        ///         "username": "UpdatedName",
        ///         "password": "NewPassword",
        ///         "email": "updatedemail@example.com"
        ///     }
        /// Exemplo de resposta JSON:
        /// 
        ///     {
        ///         "id": 1,
        ///         "username": "UpdatedName",
        ///         "email": "updatedemail@example.com"
        ///     }
        /// </remarks>
        /// <response code="200">Usuário atualizado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateUserDto userDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.Update(userDto.Username, userDto.Password, userDto.Email);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return Ok(response);
        }

        /// <summary>
        /// Deleta um usuário pelo ID especificado.
        /// </summary>
        /// <remarks>
        /// Este método define o status `isDeleted` do usuário como `true`.
        /// </remarks>
        /// <response code="204">Usuário deletado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="401">Não autorizado, faça login.</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.Delete();
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
