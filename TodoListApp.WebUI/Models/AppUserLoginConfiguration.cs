using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebUI.Models;

public class AppUserLoginConfiguration
{
    public string Token { get; set; }
}

public class LoginModel
{
    [Required(ErrorMessage = "UserToken is required")]
    public string UserToken { get; set; }
}