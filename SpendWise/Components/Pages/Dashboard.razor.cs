using SpendWise.Model;
using SpendWise.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Dashboard :ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<User> users = new List<User>();
    private string Message = string.Empty;
    private string _balance { get; set; }
    private decimal _totalInflow { get; set; }
    private decimal _totalOutflow { get; set; }
    private decimal _totalPendingDebt { get; set; }
    private decimal _totalClearedDebt { get; set; }
    private List<Debt> _pendingDebts { get; set; }
    // Highest Transaction
    private List<Transaction> _fiveHighestTransaction { get; set; }
    // Pending Debt
    private List<Debt> _debts { get; set; }
    private MudDialog ClearDebtDialog;
    private Debt _debtToClear;
    // Error Message
    private string? _errorMessage = "";

    protected override void OnInitialized()
    {
        GetAllUsers();
        GetUserBalance();
        GetTotalInflowOutflow();
        CalculateDebt();
        GetTopFiveHighestTransactions();
        GetPendingDebts();
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

    private async void GetUserBalance()
    {
        try
        {
            if (_globalState?.CurrentUser != null)
            {
                decimal totalBalance = BalanceService.GetBalance(_globalState.CurrentUser.Id);
                _balance = Utils.GetFormattedAmount(totalBalance, _globalState.CurrentUser.Currency);
            }
        }
        catch (Exception ex)
        {
            Message = "Error fetching balance.";
        }
    }

    private async void DeleteUser(string username)
    {
        bool result =  UserService.DeleteUser(username);

        Message = result ? "Successfully Deleted" : "Error Deleting User";
    }

    // total inflows, outflows
    private async void GetTotalInflowOutflow()
    {
        try
        {
            if (_globalState?.CurrentUser != null)
            {
                var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);
                if (allTransactions != null)
                {
                    _totalInflow = allTransactions
                        .Where(t => t.Type == TransactionType.Credit)
                        .Sum(t => t.Amount);
                    _totalOutflow = allTransactions
                        .Where(t => t.Type == TransactionType.Debit)
                        .Sum(t => t.Amount);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error calculating: {ex.Message}";
        }
    }

    // total pending debts, cleared debts
    private async void CalculateDebt()
    {
        try
        {
            if (_globalState?.CurrentUser != null)
            {
                var allDebts = DebtService.GetAllDebts(_globalState.CurrentUser.Id);
                if (allDebts != null)
                {
                    _totalPendingDebt = allDebts
                        .Where(t => t.Status == DebtStatus.Pending)
                        .Sum(t => t.Amount);
                    _totalClearedDebt = allDebts
                        .Where(t => t.Status == DebtStatus.Cleared)
                        .Sum(t => t.Amount);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error calculating: {ex.Message}";
        }
    }

    // pending debts
    private async void GetPendingDebts()
    {
        if (_globalState?.CurrentUser != null)
        {
            List<Debt> allDebts = DebtService.GetAllDebts(_globalState.CurrentUser.Id);
            _pendingDebts = allDebts.Where(t => t.Status == DebtStatus.Pending).ToList();
        }
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
                    GetUserBalance();
                    CalculateDebt();
                    GetPendingDebts();
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while clearing the debt.", ex);
        }
    }

    // top 5 transaction highest
    private void GetTopFiveHighestTransactions()
    {
        try
        {
            if (_globalState?.CurrentUser != null)
            {
                var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);
                if (allTransactions != null)
                {
                    _fiveHighestTransaction = allTransactions
                        .OrderByDescending(t => t.Amount) // Sort by amount in descending order
                        .Take(5) // Take the top 5
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error fetching top transactions: {ex.Message}";
        }
    }
}
