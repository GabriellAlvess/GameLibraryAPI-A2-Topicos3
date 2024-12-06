using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Mvc;

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
        /// Chama todos os usuários cadastrados
        /// </summary>
        /// <remarks>
        /// Retorno:
        /// 
        /// 
        /// </remarks>
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .ToList();

            return Ok(users);
        }

        /// <summary>
        /// Chama o usuario pelo id cadastrado
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id && !u.isDeleted)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email
                })
                .SingleOrDefault();

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            return Ok(user);
        }

        /// <summary>
        /// Cria o usuario com os dados fornecidos
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpPost]
        public IActionResult Post([FromBody] CreateUpdateUserDto userDto)
        {
            var user = new User
            {
                Id = _context.Users.Any() ? _context.Users.Max(u => u.Id) + 1 : 1,
                Username = userDto.Username,
                Password = userDto.Password,
                Email = userDto.Email,
                isDeleted = false
            };

            _context.Users.Add(user);

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Faz o atualização do usuario
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CreateUpdateUserDto userDto)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            user.Update(userDto.Username, userDto.Password, userDto.Email);
            return NoContent();
        }

        /// <summary>
        /// Deleta o usuario pelo id cadastrado
        /// </summary>
        /// <remarks>
        ///  Retorno:
        ///  
        /// </remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            user.Delete();
            return NoContent();
        }

        
    }
}
