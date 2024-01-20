using System.ComponentModel.DataAnnotations;

namespace DigitalAgency.Blazor.Front.LoginService;

public class LoginViewModel
{
    [Required]
    [StringLength(50, ErrorMessage = "Too long!")]
    public string UserName { get; set; }

    [Required] public string Password { get; set; }
}