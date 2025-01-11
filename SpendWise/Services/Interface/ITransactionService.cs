using SpendWise.Model;

namespace SpendWise.Services.Interface;

public interface ITransactionService
{
    void SaveAllTransactions(Guid userId, List<Transaction> transactions);
    List<Transaction> GetAllTransactions(Guid userId);
    List<Transaction> CreateTransaction(Guid userId, Transaction transaction);
    List<Transaction> DeleteTransaction(Guid userId, Guid id);
}
