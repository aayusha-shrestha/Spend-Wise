using Microsoft.AspNetCore.Components;
using SpendWise.Model;

namespace SpendWise.Components.Layout
{
    public partial class MainLayout
    {
        //[CascadingParameter]
        //private GlobalState _globalState { get; set; }
        private GlobalState _globalState = new GlobalState();

        private void OnInitialized()
        {
            Console.WriteLine(_globalState);
        }

        private void LogoutHandler()
        {
            if (_globalState?.CurrentUser != null)
            {
                _globalState.CurrentUser = null;
                Nav.NavigateTo("/login");
                StateHasChanged();
            }
        }
    }
}