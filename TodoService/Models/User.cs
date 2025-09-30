using System.ComponentModel.DataAnnotations;

namespace TodoService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public User() { }

        public User(string id, string name)
        {
            Id = new(id);
            Name = name;
        }
    }
}
