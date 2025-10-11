using System.ComponentModel.DataAnnotations;

namespace TodoService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        [Required] public int Version { get; set; } = 1;

        public User() { }

        public User(string id, string name, int version)
        {
            Id = new(id);
            Name = name;
            Version = version;
        }
    }
}
