using SpendWise.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Debts : ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<Debt> _debts { get; set; }
    private string _tabFilter = "All";
    private string _sortBy = "createdAt";
    private string _sortDirection = "ascending";
    // Add Debt
    private Debt _newDebt = new Debt();
    private bool IsFormValid;
    private MudForm DebtForm;
    private MudDialog AddDebtDialog;
    // Clear Debt
    private MudDialog ClearDebtDialog;
    private Debt _debtToClear;
    // private decimal _balance { get; set; }
    // Delete Debt
    private MudDialog DeleteDebtDialog;
    private Debt _debtToDelete;
    // Error Message
    private string? _errorMessage = "";

    #region OnInitialized
    protected override void OnInitialized()
    {
        if (_globalState?.CurrentUser == null)
        {
            Nav.NavigateTo("/login");
        }
        _debts = DebtService.GetAllDebts(_globalState.CurrentUser.Id);
    }
    #endregion

    // Add Debt
    private async void OpenAddTransactionDialog()
    {
        _newDebt = new Debt();
        await AddDebtDialog.ShowAsync();
    }

    private void CancelDialog()
    {
        AddDebtDialog.CloseAsync();
    }

    private void SaveTransaction()
    {
        if (!IsFormValid) return;

        try
        {
            DebtService.CreateDebt(_globalState.CurrentUser.Id, _newDebt);
            _debts.Add(_newDebt);
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
            Snackbar.Add(_errorMessage, Severity.Error);
            return;
        }
        AddDebtDialog.CloseAsync();
    }

    // Clear Debt
    private async void OpenClearDebtDialog(Debt debt)
    {
        _debtToClear = debt;
        await ClearDebtDialog.ShowAsync();
    }

    private void CancelClearDialog()
    {
        ClearDebtDialog.CloseAsync();
    }

    private void ClearDebt()
    {
        try
        {
            if (_debtToClear == null) return;

            if (_globalState.CurrentUser != null)
            {
                decimal totalBalance = BalanceService.GetBalance(_globalState.CurrentUser.Id);

                if (_debtToClear.Amount > totalBalance)
                {
                    _errorMessage = "You do not have sufficient balance to clear the debt.";
                    Snackbar.Add(_errorMessage, Severity.Error);
                    return;
                }
                else
                {
                    _debts = DebtService.ClearDebt(_globalState.CurrentUser.Id, _debtToClear.Id);
                    ClearDebtDialog.CloseAsync();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while clearing the debt.", ex);
        }
    }

    // Delete Debt
    private async void OpenDeleteDebtDialog(Debt debt)
    {
        _debtToDelete = debt;
        await DeleteDebtDialog.ShowAsync();
    }

    private void CancelDeleteDialog()
    {
        DeleteDebtDialog.CloseAsync();
    }

    private void DeleteDebt()
    {
        if (_debtToDelete == null) return;
        _debts = DebtService.DeleteDebt(_globalState.CurrentUser.Id, _debtToDelete.Id);
        DeleteDebtDialog.CloseAsync();
    }
}
