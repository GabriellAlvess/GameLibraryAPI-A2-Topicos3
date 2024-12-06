using GameLibraryAPI.DTO;

namespace GameLibraryAPI.Dtos
{
    public class CreateUpdatGameDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DeveloperId { get; set; }
        public List<int> GenreIds { get; set; } 
    }

    public class GameResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DeveloperResponseDto Developer { get; set; }
        public List<GenreResponseDto> Genres { get; set; }
    }
}