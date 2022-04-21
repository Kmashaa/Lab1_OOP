namespace Lab1.Models;

public class Loan
{
    public Loan(IReadOnlyList<string> data)
    {
        Id = Convert.ToInt32(data[0]);
        UserId = Convert.ToInt32(data[1]);
        AccId = Convert.ToInt32(data[2]);
        Sum = data[3];
        BankSum = data[4];
        Time = Convert.ToInt32(data[5]);
        MonPayment = data[6];
        Payments = Convert.ToInt32(data[7]);
        Debt = data[8];
        Date = data[9];
    }

    public int Id { get; }
    public int UserId { get; }
    public int AccId { get; }
    public string Sum { get; }
    public string BankSum { get; }
    public int Time { get; }
    public string MonPayment { get; }
    public int Payments { get; }
    public string Debt { get; }
    public string Date { get; }

    public override string ToString()
    {
        return $"Sum: {Sum,-10}|  " +
               $"Account ID: {AccId,-5}|  " +
               $"Month Payment: {MonPayment,-15}\n";
    }
}