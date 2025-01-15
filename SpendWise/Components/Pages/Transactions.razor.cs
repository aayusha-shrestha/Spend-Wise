using SpendWise.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SpendWise.Components.Pages;

public partial class Transactions : ComponentBase
{
    [CascadingParameter]
    private GlobalState _globalState { get; set; }
    private List<Transaction> _transactions { get; set; }
    private string _tabFilter = "All";
    private string _sortBy = "createdAt";
    private string _sortDirection = "ascending";
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

    protected override void OnInitialized()
    {
        if (_globalState?.CurrentUser == null)
        {
            Nav.NavigateTo("/login");
        }
        _transactions = TransactionService.GetAllTransactions(_globalState.CurrentUser.Id);
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
        if (!IsFormValid) return;

        TransactionService.CreateTransaction(_globalState.CurrentUser.Id, _newTransaction);
        _transactions.Add(_newTransaction);
        AddTransactionDialog.CloseAsync();
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
