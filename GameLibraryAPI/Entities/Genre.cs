using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace GameLibraryAPI.Entities
{
    public class Genre
    {
        public Genre()
        {
            IsDeleted = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }


        public void Update(string name)
        {
            Name = name;
        }

        public void Delete()
        {
            IsDeleted = true;
        }


    }
}