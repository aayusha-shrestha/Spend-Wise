namespace SpendWise.Model;

public class Debt: Transaction
{
    public string SourceOfDebt { get; set; }
    public DateTime? DueDate { get; set; } = DateTime.Today;
    public DebtStatus Status { get; set; } = 0;
    public TransactionType Type { get; set; } = TransactionType.Debt;
}