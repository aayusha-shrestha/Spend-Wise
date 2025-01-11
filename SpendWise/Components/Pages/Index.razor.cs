using SpendWise.Model;
using Microsoft.AspNetCore.Components;

namespace SpendWise.Components.Pages
{
    public partial class Index :ComponentBase
    {

        [CascadingParameter]
        private GlobalState _globalState { get; set; }

        protected override void OnInitialized()
        {

            if (_globalState?.CurrentUser == null)
            {
                Nav.NavigateTo("/register");
            }
            else
            {
                Nav.NavigateTo("/dashboard");
            }

        }
    }
}