using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoService.Models
{
    public class TodoItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = null!;
        public bool IsDone { get; set; } = false;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public TodoItem() { }

        public TodoItem(string name, User user)
        {
            Name = name;
            User = user;
            UserId = user.Id;
        }
    }
}
