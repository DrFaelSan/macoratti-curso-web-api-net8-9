using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class RegisterModel
{
    [Required(ErrorMessage = "User Name is Required")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email is Required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    public string? Password { get; set; }
}
