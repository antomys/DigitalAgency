using System;
using System.Threading.Tasks;
using DigitalAgency.Blazor.Front.Infrastructure;
using Microsoft.AspNetCore.Components;

namespace DigitalAgency.Blazor.Front.LoginService;

public class LoginModel : ComponentBase
{
    public LoginModel()
    {
        LoginData = new LoginViewModel();
    }

    public LoginViewModel LoginData { get; set; }
    [Inject] public ILocalStorageService LocalStorageService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    protected async Task LoginAsync()
    {
        SecurityToken token = new SecurityToken {
            AccessToken = LoginData.Password, UserName = LoginData.UserName, ExpiredAt = DateTime.UtcNow.AddMinutes(30),
        };
        await LocalStorageService.SetAsync(nameof(SecurityToken), token);

        NavigationManager.NavigateTo("/", true);
    }
}

public class SecurityToken
{
    public string UserName { get; set; }
    public string AccessToken { get; set; }
    public DateTime ExpiredAt { get; set; }
}