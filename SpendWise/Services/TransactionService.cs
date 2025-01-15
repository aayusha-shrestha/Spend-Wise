using SpendWise.Model;
using SpendWise.Services.Interface;
using SpendWise.Utilities;
using System.Text.Json;

namespace SpendWise.Services;

public class TransactionService: ITransactionService
{
    public void SaveAllTransactions(Guid userId, List<Transaction> transactions)
    {
        string transactionsFilePath = Utils.GetTransactionsFilePath(userId);

        var json = JsonSerializer.Serialize(transactions, new JsonSerializerOptions { WriteIndented = true });

        // Save the transactions to json. If the file doesnt exist, File.WriteAllText will create it
        File.WriteAllText(transactionsFilePath, json);
    }
    public List<Transaction> GetAllTransactions(Guid userId)
    {
        string transactionsFilePath = Utils.GetTransactionsFilePath(userId);
        if (!File.Exists(transactionsFilePath))
        {
            return new List<Transaction>();
        }

        var json = File.ReadAllText(transactionsFilePath);

        return JsonSerializer.Deserialize<List<Transaction>>(json);
    }

    public List<Transaction> CreateTransaction(Guid userId, Transaction transaction)
    {
        //if (dueDate < DateTime.Today)
        //{
        //    throw new Exception("Due date must be in the future.");
        //}
        // Get all transactions. If the file doesn't exist, it will return an empty list
        List<Transaction> transactions = GetAllTransactions(userId);

        transactions.Add(transaction);

        SaveAllTransactions(userId, transactions);
        return transactions;
    }
    public List<Transaction> DeleteTransaction(Guid userId, Guid id)
    {
        List<Transaction> transactions = GetAllTransactions(userId);
        Transaction transaction = transactions.FirstOrDefault(x => x.Id == id);

        if (transaction == null)
        {
            throw new Exception("We couldn't find the transaction you're looking for.");
        }

        transactions.Remove(transaction);
        SaveAllTransactions(userId, transactions);
        return transactions;
    }
}
