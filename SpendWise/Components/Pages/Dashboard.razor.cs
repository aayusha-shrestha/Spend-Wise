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
    private List<Debt> _pendingDebts { get; set; }
    // Dashboard Statistics
    private string _balance { get; set; }
    private string _totalInflow { get; set; }
    private string _totalOutflow { get; set; }
    private string _totalDebt { get; set; }
    private string _totalPendingDebt { get; set; }
    private string _totalClearedDebt { get; set; }
    private decimal _totalTransactionCount { get; set; }
    private int Index = -1;
    public List<ChartSeries> Series = new List<ChartSeries>();
    public string[] XAxisLabels = { "Inflow", "Outflow", "Debt" };
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
        GenerateChartData();
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
                    decimal totalDebt = totalPendingDebt + totalClearedDebt;
                    _totalPendingDebt = Utils.GetFormattedAmount(totalPendingDebt, _globalState.CurrentUser.Currency);
                    _totalClearedDebt = Utils.GetFormattedAmount(totalClearedDebt, _globalState.CurrentUser.Currency);
                    _totalDebt = Utils.GetFormattedAmount(totalDebt, _globalState.CurrentUser.Currency);
                }
            }
        }
        catch (Exception ex)
        {
            Message = $"Error calculating: {ex.Message}";
        }
    }

    // Gennerate Chart Data
    private void GenerateChartData()
    {
        if (_globalState?.CurrentUser != null)
        {
            // Fetch all transactions for the current user
            var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);

            if (allTransactions != null)
            {
                // Calculate Inflows
                var inflowTransactions = allTransactions.Where(t => t.Type == TransactionType.Credit).ToList();
                var inflowHighest = inflowTransactions.Max(t => t.Amount);
                var inflowLowest = inflowTransactions.Min(t => t.Amount);
                var inflowTotal = inflowTransactions.Sum(t => t.Amount);

                // Calculate Outflows
                var outflowTransactions = allTransactions.Where(t => t.Type == TransactionType.Debit).ToList();
                var outflowHighest = outflowTransactions.Max(t => t.Amount);
                var outflowLowest = outflowTransactions.Min(t => t.Amount);
                var outflowTotal = outflowTransactions.Sum(t => t.Amount);

                // Calculate Debts
                var allDebts = DebtService.GetAllDebts(_globalState.CurrentUser.Id);
                var debtHighest = allDebts?.Max(d => d.Amount) ?? 0;
                var debtLowest = allDebts?.Min(d => d.Amount) ?? 0;
                var debtTotal = allDebts?.Sum(d => d.Amount) ?? 0;

                // Update chart data
                Series = new List<ChartSeries>
            {
                new ChartSeries { Name = "Highest", Data = new double[] { (double)inflowHighest, (double)outflowHighest, (double)debtHighest } },
                new ChartSeries { Name = "Lowest", Data = new double[] { (double)inflowLowest, (double)outflowLowest, (double)debtLowest } },
                new ChartSeries { Name = "Total", Data = new double[] { (double)inflowTotal, (double)outflowTotal, (double)debtTotal } },
            };

                // Update X-axis labels
                XAxisLabels = new[] { "Inflow", "Outflow", "Debt" };
            }
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
