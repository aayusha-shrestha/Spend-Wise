using SpendWise.Model;

namespace SpendWise.Services.Interface;

public interface IDebtService
{
    void SaveAllDebts(Guid userId, List<Debt> debts);
    List<Debt> GetAllDebts(Guid userId);
    List<Debt> CreateDebt(Guid userId, Debt debt);
    List<Debt> ClearDebt(Guid userId, Guid id);
    List<Debt> DeleteDebt(Guid userId, Guid id);
}
