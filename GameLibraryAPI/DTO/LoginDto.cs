using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.DTO
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
