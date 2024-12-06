namespace GameLibraryAPI.DTO
{
    public class CreateUpdateGenreDto
    {
        public string Name { get; set; }
    }

    public class GenreResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class GenreResponseWithoutDeletedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
