using System.ComponentModel.DataAnnotations;

namespace BackendForIDO.Models
{
   public class User
{
    // Primary key for the User entity
    [Key]
    public int Id { get; set; }

    // Email property with data validation attributes
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    // PasswordHash property with data validation attribute
    public string Password { get; set; }

    // Navigation property for related tasks
    public List<TaskEntity> Tasks { get; set; }
}

}
