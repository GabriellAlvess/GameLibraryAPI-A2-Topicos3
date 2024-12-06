
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GameLibraryAPI.Entities
{
    public class Games
    {
        public Games()
        {
            Genres = new List<Genre>();
            Reviews = new List<Review>();
            isDeleted = false;
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Developer Developer { get; set; }

        [Required]
        public List<Genre> Genres { get; set; } 
        public List<Review> Reviews { get; set; }
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0.0;
        public bool isDeleted { get; set; }

        /// <summary>
        /// Atualiza as propriedades do jogo.
        /// </summary>
        public void Update(string title, string description, Developer developer, List<Genre> genres)
        { 
            Title = title;
            Description = description;
            Developer = developer;
            // Substitui a lista de gêneros existente
            Genres = genres ?? new List<Genre>();
        }

        public void Delete()
        {
            isDeleted = true;
        }

    }
}
