namespace Lab1.Models;

public class Staff : User
{
    public Staff(IReadOnlyList<string> data)
    {
        Id = Convert.ToInt32(data[0]);
        AccessLevel = data[1];
    }

    private string AccessLevel { get; }

    public override List<string> GetInfo()
    {
        return new List<string> {Id.ToString(), AccessLevel};
    }
}