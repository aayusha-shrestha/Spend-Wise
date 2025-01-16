using SpendWise.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Tags : ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<Tag> _tags = new List<Tag>();
    // Add Tag
    private Tag _newTag = new Tag();
    private bool IsFormValid;
    private MudForm TagForm;
    private MudDialog AddTagDialog;
    // Error Message
    private string? _errorMessage = "";

    protected override void OnInitialized()
    {
        GetUserTags();
    }

    // Get All User Tags
    private void GetUserTags()
    {
        if (_globalState?.CurrentUser != null)
        {
            _tags = TagService.GetUserTags(_globalState.CurrentUser.Id);
        }
    }

    // Add Tag
    private async void OpenAddTagDialog()
    {
        _newTag = new Tag();
        await AddTagDialog.ShowAsync();
    }

    private void CancelDialog()
    {
        AddTagDialog.CloseAsync();
    }

    private void SaveTag()
    {
        if (!IsFormValid) return;

        try
        {
            if (_globalState?.CurrentUser != null)
            {
                TagService.AddCustomTag(_globalState.CurrentUser.Id, _newTag);
                _tags.Add(_newTag);
                GetUserTags();
            }
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
            Snackbar.Add(_errorMessage, Severity.Error);
            return;
        }
        AddTagDialog.CloseAsync();
    }
}
