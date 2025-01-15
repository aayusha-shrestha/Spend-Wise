using SpendWise.Model;
using SpendWise.Services.Interface;
using SpendWise.Utilities;
using System.Text.Json;

namespace SpendWise.Services;

public class DebtService : IDebtService
{
    public void SaveAllDebts(Guid userId, List<Debt> debts)
    {
        string debtsFilePath = Utils.GetDebtsFilePath(userId);

        var json = JsonSerializer.Serialize(debts, new JsonSerializerOptions { WriteIndented = true });

        // Save the transactions to json. If the file doesnt exist, File.WriteAllText will create it
        File.WriteAllText(debtsFilePath, json);
    }
    public List<Debt> GetAllDebts(Guid userId)
    {
        string debtsFilePath = Utils.GetDebtsFilePath(userId);
        if (!File.Exists(debtsFilePath))
        {
            return new List<Debt>();
        }

        var json = File.ReadAllText(debtsFilePath);

        return JsonSerializer.Deserialize<List<Debt>>(json);
    }

    public List<Debt> CreateDebt(Guid userId, Debt debt)
    {
        //if (dueDate < DateTime.Today)
        //{
        //    throw new Exception("Due date must be in the future.");
        //}
        // Get all transactions. If the file doesn't exist, it will return an empty list
        List<Debt> debts = GetAllDebts(userId);

        debts.Add(debt);

        SaveAllDebts(userId, debts);
        return debts;
    }

    // Update the status of the debt to 'Cleared'
    public List<Debt> ClearDebt(Guid userId, Guid id)
    {
        List<Debt> debts = GetAllDebts(userId);
        Debt debt = debts.FirstOrDefault(x => x.Id == id);

        if (debt == null)
        {
            throw new Exception("We couldn't find the debt you're looking for.");
        }

        debt.Status = DebtStatus.Cleared;
        SaveAllDebts(userId, debts);
        return debts;
    }
    public List<Debt> DeleteDebt(Guid userId, Guid id)
    {
        List<Debt> debts = GetAllDebts(userId);
        Debt debt = debts.FirstOrDefault(x => x.Id == id);

        if (debt == null)
        {
            throw new Exception("We couldn't find the debt you're looking for.");
        }

        debts.Remove(debt);
        SaveAllDebts(userId, debts);
        return debts;
    }
}

