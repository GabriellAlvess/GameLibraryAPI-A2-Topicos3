using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.DTO
{
    public class CreateUpdateUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

    }
}
