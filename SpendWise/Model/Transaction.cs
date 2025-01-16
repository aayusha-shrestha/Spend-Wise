namespace SpendWise.Model;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public List<string> Tags { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
