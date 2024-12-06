using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public Games Game { get; set; }

        [Required]
        public User User { get; set; }

        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // 1 to 5



    }
}