namespace Lab1.Models;

public class Bank : Company
{
    public Bank(int id, IReadOnlyList<string> data, List<int>? clients) : base(Convert.ToInt32(id), data)
    {
        Clients = clients;
    }

    private List<int>? Clients { get; }
}