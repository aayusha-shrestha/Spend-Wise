namespace SpendWise.Model;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; }
    public string Password { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

