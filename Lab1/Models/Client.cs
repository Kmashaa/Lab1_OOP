namespace Lab1.Models;

public class Client : User
{
    public Client(int id, IReadOnlyList<string> data)
    {
        Id = id;
        Name = data[0];
        PassNum = data[1];
        PassId = data[2];
        Phone = data[3];
        Email = data[4];
    }

    private string PassNum { get; }
    private string PassId { get; }
    private string Phone { get; }
    private string Email { get; }

    public override List<string> GetInfo()
    {
        return new List<string> {Convert.ToString(Id), Name!, PassNum, PassId, Phone, Email};
    }

    public override string ToString()
    {
        return $"• Name: {Name}\n" +
               $"• Passport №: {PassNum}\n" +
               $"• Passport ID: {PassId}\n" +
               $"• Phone Number: +{Phone}\n" +
               $"• Email: {Email}";
    }
}