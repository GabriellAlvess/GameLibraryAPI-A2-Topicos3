using GameLibraryAPI.Dtos;
using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.DTO
{
    public class AddReviewDto
    {
        [MaxLength(255)]
        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }

    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public UserResponseDto user { get; set; }
        public GameResponseDto game { get; set; }
    }


}
