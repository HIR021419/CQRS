using System.ComponentModel.DataAnnotations;

namespace UserService.Models;

public class AddUserDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Name { get; set; } = null!;
}