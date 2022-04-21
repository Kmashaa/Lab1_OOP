namespace Lab1.Models;

public class Account
{
    public Account(IReadOnlyList<string> data)
    {
        Id = Convert.ToInt32(data[0]);
        BankId = Convert.ToInt32(data[1]);
        ClientId = data[2];
        Balance = data[3];
        Currency = data[4];
    }

    public int Id { get; }
    public int BankId { get; }
    public string ClientId { get; }
    public string? Balance { get; }
    public string Currency { get; }

    public override string ToString()
    {
        return $"ID: {Id,-4} |  " +
               $"Balance: {Balance,-12}  |  " +
               $"Currency: {Currency}\n";
    }
}