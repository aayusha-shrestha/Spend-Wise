using SpendWise.Model;
using SpendWise.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Transactions : ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<Transaction> _transactions { get; set; }
    private string _balance { get; set; }

    // Add Transaction
    private Transaction _newTransaction = new Transaction();
    private bool IsFormValid;
    private MudForm TransactionForm;
    private MudDialog AddTransactionDialog;
    // Delete Transaction
    private MudDialog DeleteTransactionDialog;
    private Transaction _transactionToDelete;
    // Date Range
    private PickerVariant _variant = PickerVariant.Dialog;
    private DateRange _dateRange { get; set; }
    // Error Message
    private string? _errorMessage = "";

    protected override void OnInitialized()
    {
        if (_globalState?.CurrentUser == null)
        {
            Nav.NavigateTo("/login");
        }
        _transactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);
        GetUserBalance();
    }

    // Get current balance of user
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
            _errorMessage = "Error fetching balance.";
        }
    }

    // Search by title
    private void SearchTransactionTitle(ChangeEventArgs e)
    {
        var searchTerm = e.Value.ToString();
        var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            //ToList() method converts the filtered collection IEnumerable<Transaction> into a List<Transaction>
            _transactions = allTransactions.Where(t =>
                t.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
        }
        else
        {
            _transactions = allTransactions;
        }
    }

    // Filter by Transaction Type
    private void FilterByTransactionType(ChangeEventArgs e)
    {
        var selectedType = e.Value.ToString();
        var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);

        if (!string.IsNullOrEmpty(selectedType))
        {
            _transactions = allTransactions.Where(t => t.Type.ToString() == selectedType).ToList();
        }
        else
        {
            _transactions = allTransactions;
        }
    }

    // Filter by Tag
    private void FilterByTag(ChangeEventArgs e)
    {
        var selectedTag = e.Value.ToString();
        var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);

        if (!string.IsNullOrEmpty(selectedTag))
        {
            _transactions = allTransactions.Where(t => t.Tags.ToString() == selectedTag).ToList();
        }
        else
        {
            _transactions = allTransactions;
        }
    }

    // Filter by Specific Date
    private void FilterBySpecificDate(ChangeEventArgs e)
    {
        var allTransactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);

        if (e.Value.ToString() != "")
        {
            var selectedDate = DateTime.Parse(e.Value.ToString());
            _transactions = allTransactions.Where(t => t.CreatedAt.Date == selectedDate.Date).ToList();
        }
        else
        {
            _transactions = allTransactions;
        }
    }

    private void OnDateRangeChanged()
    {
        if (_dateRange.Start.HasValue && _dateRange.End.HasValue)
        {
            _transactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id)
                .Where(t => t.CreatedAt.Date >= _dateRange.Start.Value.Date && t.CreatedAt.Date <= _dateRange.End.Value.Date)
                .ToList();
        }
        else
        {
            _transactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);
        }
    }

    // Add Transaction
    private async void OpenAddTransactionDialog()
    {
        _newTransaction = new Transaction();
        await AddTransactionDialog.ShowAsync();
    }

    private void CancelDialog()
    {
        AddTransactionDialog.CloseAsync();
    }

    private void SaveTransaction()
    {
        try
        {
            if (!IsFormValid) return;

            if (_newTransaction == null) return;

            if (_globalState.CurrentUser != null)
            {
                decimal totalBalance = BalanceService.GetBalance(_globalState.CurrentUser.Id);

                if (_newTransaction.Type == TransactionType.Debit)
                {
                    // Validate balance for debit transactions
                    if (_newTransaction.Amount > totalBalance)
                    {
                        _errorMessage = "You do not have sufficient balance for this debit transaction.";
                        Snackbar.Add(_errorMessage, Severity.Error);
                        return;
                    }
                    else
                    {
                        TransactionService.CreateTransaction(_globalState.CurrentUser.Id, _newTransaction);
                        _transactions.Add(_newTransaction);
                    }
                }
                else if (_newTransaction.Type == TransactionType.Credit)
                {
                    // Add the transaction for credit transactions
                    TransactionService.CreateTransaction(_globalState.CurrentUser.Id, _newTransaction);
                    _transactions.Add(_newTransaction);
                }
                GetUserBalance();
                AddTransactionDialog.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while processing the transaction.", ex);
        }
    }


    // Delete Transaction
    private async void OpenDeleteTransactionDialog(Transaction transaction)
    {
        _transactionToDelete = transaction;
        await DeleteTransactionDialog.ShowAsync();
    }

    private void CancelDeleteDialog()
    {
        DeleteTransactionDialog.CloseAsync();
    }

    private void DeleteTransaction()
    {
        if (_transactionToDelete == null) return;
        _transactions = TransactionService.DeleteTransaction(_globalState.CurrentUser.Id, _transactionToDelete.Id);
        DeleteTransactionDialog.CloseAsync();
    }
}
