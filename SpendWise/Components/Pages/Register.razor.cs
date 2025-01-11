using SpendWise.Model;

namespace SpendWise.Components.Pages;
public partial class Register 
{
    private string? Message;

    private User Users {  get; set; } = new User();

    private async void RegisterUser()
    {
        if (UserService.Register(Users))
        {
            Message = "User registered successfully!";
            Nav.NavigateTo("/login");
        }
        else
        {
            Message = "Username already exists.";
        }
    }

    private async void HandleLogin()
    {
        Nav.NavigateTo("/login");
    }
}