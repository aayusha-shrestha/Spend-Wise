using SpendWise.Model;
using Microsoft.AspNetCore.Components;

namespace SpendWise.Components.Pages;

public partial class Dashboard :ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<User> users = new List<User>();

    private string Message = string.Empty;

    private bool IslogOut {  get; set; }  = false;
    protected override void OnInitialized()
    {
        GetAllUsers();
    }
    private void Logout()
    {
        Nav.NavigateTo("/login");
        _globalState.CurrentUser = null;
    }

    private async Task<List<User>> GetAllUsers()
    {
        try
        {
            users =  UserService.GetAllUsers();

            return users;

        }
        catch (Exception ex)
        {
            throw new Exception();
        }
    }

    private async Task DeleteUser(string username)
    {
        bool result =  UserService.DeleteUser(username);

        Message = result ? "Successfully Deleted" : "Error Deleting User";
    }

    private void ShowLogoutConfirmation()
    {
        IslogOut = true;
    }

    private void HideLogoutConfirmation()
    {
        IslogOut = false;
    }
}
