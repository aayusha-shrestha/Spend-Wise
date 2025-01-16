using SpendWise.Model;
using SpendWise.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Dashboard :ComponentBase
{
    private int Index = -1;
    public List<ChartSeries> Series = new List<ChartSeries>();
    public string[] XAxisLabels = { "Inflow", "Outflow", "Debt" };

    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<User> users = new List<User>();
    private string Message = string.Empty;
    private List<Debt> _pendingDebts { get; set; }
    // Dashboard Statistics
    private string _balance { get; set; }
    private string _totalInflow { get; set; }
    private string _totalOutflow { get; set; }
    private string _totalPendingDebt { get; set; }
    private string _totalClearedDebt { get; set; }
    private decimal _totalTransactionCount { get; set; }
    // Highest, Lowest, Total Transaction and Debt

    // Five Highest Transaction
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
        Series = new List<ChartSeries>
        {
            new ChartSeries { Name = "Highest", Data = new double[] { (double)920, (double)960, (double)320 } },
            new ChartSeries { Name = "Lowest", Data = new double[] { (double)440, (double)160, (double)50 } },
            new ChartSeries { Name = "Total", Data = new double[] { (double)1360, (double)1120, (double)370 } },
        };
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
                    decimal totalInflow = allTransactions
                        .Where(t => t.Type == TransactionType.Credit)
                        .Sum(t => t.Amount);
                    decimal totalOutflow = allTransactions
                        .Where(t => t.Type == TransactionType.Debit)
                        .Sum(t => t.Amount);
                    _totalTransactionCount = allTransactions.Count();

                    _totalInflow = Utils.GetFormattedAmount(totalInflow, _globalState.CurrentUser.Currency);
                    _totalOutflow = Utils.GetFormattedAmount(totalOutflow, _globalState.CurrentUser.Currency);
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
                    decimal totalPendingDebt = allDebts
                        .Where(t => t.Status == DebtStatus.Pending)
                        .Sum(t => t.Amount);
                    decimal totalClearedDebt = allDebts
                        .Where(t => t.Status == DebtStatus.Cleared)
                        .Sum(t => t.Amount);
                    _totalPendingDebt = Utils.GetFormattedAmount(totalPendingDebt, _globalState.CurrentUser.Currency);
                    _totalClearedDebt = Utils.GetFormattedAmount(totalClearedDebt, _globalState.CurrentUser.Currency);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error calculating: {ex.Message}";
        }
    }

    // CalculateTransactionAndDebtStats

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
