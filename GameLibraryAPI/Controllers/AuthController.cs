using GameLibraryAPI.DTO;
using GameLibraryAPI.Entities;
using GameLibraryAPI.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameLibraryAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly GamesLibraryDbContext _context;

        public AuthController(IConfiguration configuration, GamesLibraryDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Realiza o login e retorna um token JWT.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     POST
        ///     {
        ///         "email": "johndoe@example.com",
        ///         "password": "1234"
        ///     }
        /// </remarks>
        /// <response code="200">Retorna o token JWT.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Valida o usuário
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            // Gera o token JWT
            var token = GenerateJwtToken(user.Email);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Gera um token JWT.
        /// </summary>
        private string GenerateJwtToken(string email)
        {
            var key = Encoding.UTF8.GetBytes("ChaveSecretaParaJWTMuitoSeguraEGrande12345678");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
