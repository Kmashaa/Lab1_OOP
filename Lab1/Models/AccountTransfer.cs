namespace Lab1.Models;

public class AccountTransfer
{
    public AccountTransfer(IReadOnlyList<Account> data, string sum, int id = -1)
    {
        Id = id;

        SendId = data[0].Id;
        GetId = data[1].Id;

        BankSendId = data[0].BankId;
        BankGetId = data[1].BankId;

        Currency = data[0].Currency;
        Sum = Convert.ToDouble(sum).ToString("0.##");
    }

    public AccountTransfer(int id, List<Account?> data, string sum)
    {
        Id = id;

        SendId = data[0] != null ? data[0]!.Id : -1;
        GetId = data[1] != null ? data[1]!.Id : -1;

        BankSendId = data[0] != null ? data[0]!.BankId : -1;
        BankGetId = data[1] != null ? data[1]!.BankId : -1;

        Currency = data[0] == null ? data[1]!.Currency : data[0]!.Currency;
        Sum = Convert.ToDouble(sum).ToString("0.##");
    }

    public int Id { get; }
    public int SendId { get; set; }
    public int GetId { get; set; }
    public int BankSendId { get; set; }
    public int BankGetId { get; set; }
    public string Currency { get; }
    public string Sum { get; }

    public override string ToString()
    {
        return $"Transfer ID: {Id,-4} | " +
               $"{(SendId != -1 ? $"Acc-S ID: {SendId,-3}" : "    Credit   ")} | " +
               $"{(GetId != -1 ? $"Acc-G ID: {GetId,-3}" : "   Debit     ")} | " +
               $"{(BankSendId != -1 ? $"Bank-S ID: {BankSendId,-1}" : "   Credit   ")} | " +
               $"{(BankGetId != -1 ? $"Bank-G ID: {BankGetId,-1}" : "   Debit    ")} | " +
               $"Sum: {Currency}{Sum}\n";
    }
}