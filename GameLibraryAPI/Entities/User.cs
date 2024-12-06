using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.Entities
{
    public class User
    {
        public User()
        {
            Library = new List<Games>();
        }

        public int Id { get; set; }
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public List<Games> Library { get; set; }
        public bool isDeleted { get; set; }

        public void Update(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public void Delete() {
            isDeleted = true;
        }
    }
}
