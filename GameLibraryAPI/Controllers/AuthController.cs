using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Realiza o login e retorna um token JWT.
        /// </summary>
        /// <remarks>
        /// Exemplo de corpo de requisição:
        /// 
        ///     POST
        ///     {
        ///         "username": "admin",
        ///         "password": "1234"
        ///     }
        /// </remarks>
        /// <response code="200">Retorna o token JWT.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            // Autenticação fictícia para fins didáticos
            if (loginDto.Username == "admin" && loginDto.Password == "1234")
            {
                var token = GenerateJwtToken(loginDto.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }

        /// <summary>
        /// Gera um token JWT.
        /// </summary>
        private string GenerateJwtToken(string username)
        {
            var key = Encoding.UTF8.GetBytes("ChaveSecretaParaDidatica12345"); // Use a mesma chave definida no Program.cs
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
