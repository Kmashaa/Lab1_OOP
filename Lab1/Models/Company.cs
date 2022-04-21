namespace Lab1.Models;

public class Company : User
{
    public Company(int id, IReadOnlyList<string> data)
    {
        Id = id;
        Name = data[0];
        Type = data[1];
        Address = data[2];
        Unp = data[3];
        Bic = data[4];
    }

    public string Type { get; }
    public string Unp { get; }
    public string Bic { get; }
    public string Address { get; }

    public override List<string> GetInfo()
    {
        return new List<string> {Convert.ToString(Id), Name!, Type, Unp, Bic, Address};
    }

    public override string ToString()
    {
        return $"• Company legal name: {Name}\n" +
               $"• Company type: {Type}\n" +
               $"• Company legal address: {Address}\n" +
               $"• Company UNP: {Unp}\n" +
               $"• Company BIC: {Bic}";
    }
}