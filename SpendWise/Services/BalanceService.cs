using SpendWise.Services.Interface;
using SpendWise.Model;

namespace SpendWise.Services;
public class BalanceService: IBalanceService
{
    private readonly ITransactionService _transactionService;
    private readonly IDebtService _debtService;

    public BalanceService(ITransactionService transactionService, IDebtService debtService)
    {
        _transactionService = transactionService;
        _debtService = debtService;
    }

    public decimal GetBalance(Guid userId)
    {
        // Get all transactions and debts
        var transactions = _transactionService.GetAllTransactions(userId) ?? new List<Transaction>();
        var debts = _debtService.GetAllDebts(userId) ?? new List<Debt>();

        decimal totalBalance = 0;

        // Calculate balance from transactions
        foreach (var transaction in transactions)
        {
            switch (transaction.Type)
            {
                case TransactionType.Credit:
                    totalBalance += transaction.Amount;
                    break;

                case TransactionType.Debit:
                    totalBalance -= transaction.Amount;
                    break;
            }
        }

        Console.WriteLine($"=====totalBalance {totalBalance}");

        // Calculate balance from debts
        foreach (var debt in debts)
        {
            switch (debt.Status)
            {
                case DebtStatus.Cleared:
                    totalBalance -= debt.Amount;
                    break;

                case DebtStatus.Pending:
                    totalBalance += debt.Amount;
                    break;
            }
        }
        return totalBalance;
    }
}
