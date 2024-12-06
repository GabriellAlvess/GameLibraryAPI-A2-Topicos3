namespace GameLibraryAPI.DTO
{
    public class CreateUpdateDeveloperDto
    {
        public string Name { get; set; }
    }

    public class DeveloperResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
