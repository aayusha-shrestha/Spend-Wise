using SpendWise.Model;
using Microsoft.AspNetCore.Components;

namespace SpendWise.Components.Pages;

public partial class Login: ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private string? _errorMessage = "";
    public User User { get; set; } = new();
    protected override void OnInitialized()
    {
        if (_globalState == null)
        {
            _errorMessage = "Global state is null.";
            return;
        }

        try
        {
            _globalState.CurrentUser = null;
            _errorMessage = "";
        }
        catch (Exception e)
        {
            _errorMessage = e.Message;
        }
    }

    private async void HandleLogin()
    {
        try
        {
            _errorMessage = "";
            _globalState.CurrentUser = UserService.Login(User);
            if (_globalState.CurrentUser != null)
            {
                Console.WriteLine(_globalState.CurrentUser);
                Nav.NavigateTo("/dashboard");
            }
        }
        catch (Exception e)
        {
            _errorMessage = e.Message;
        }
    }

    private async void HandleRegister()
    {
        Nav.NavigateTo("/register");
    }
}