using System.ComponentModel.DataAnnotations;

namespace BackendForIDO.Models
{
    public class LoginRequest
    {
       [Required]
       public string UsernameOrEmail { get; set; } = string.Empty;
       // Represents the username or email entered by the user during login
       // The [Required] attribute indicates that this property must have a value

       [Required]
       public string Password { get; set; } = string.Empty;
       // Represents the password entered by the user during login
       // The [Required] attribute indicates that this property must have a value

    }
}

