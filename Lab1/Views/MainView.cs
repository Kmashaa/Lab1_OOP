using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public static class MainView
{
    public static BankChoiceMenu? BankM;
    public static List<Bank>? Banks { get; private set; }
    public static List<AuthMenu?> AuthM { get; private set; } = null!;
    public static List<ClientMenu?> ClientM { get; private set; } = null!;

    public static void Initialize()
    {
        Banks = MainController.Initialize().Result;

        if (Banks == null)
        {
            Console.WriteLine("⚠ No Banks found: possible DB error");
            return;
        }

        AuthM = new List<AuthMenu?>();
        ClientM = new List<ClientMenu?>();
        BankM = new BankChoiceMenu();

        for (var i = 0; i < Banks.Count; i++)
        {
            AuthM.Add(new AuthMenu());
            ClientM.Add(new ClientMenu());
        }


        MenuController.Print(BankM);
        BankM.Operate();

        Console.Clear();
    }
}