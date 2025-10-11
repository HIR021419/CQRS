using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;

        public int Version { get; set; } = 1;


        public User() { }

        public User(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
