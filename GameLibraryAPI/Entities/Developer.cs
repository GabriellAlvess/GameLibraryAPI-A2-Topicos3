using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.Entities
{
    public class Developer
    {

        public Developer()
        {
            isDeleted = false;
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool isDeleted { get; set; }

        public void Update(string name)
        {
            Name = name;
        }
        public void Delete()
        {
            isDeleted = true;
        }
    }
}