namespace SpendWise.Model;

public class Debt: Transaction
{
    public SourceOfDebt SourceOfDebt { get; set; }
    public DateTime? DueDate { get; set; } = DateTime.Today;
    public DebtStatus Status { get; set; } = 0;
}