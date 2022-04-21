using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class ClientMenu : Menu
{
    public ClientMenu() : base
    (
        new List<string>
        {
            "Show my personal info",
            "Manage my accounts",
            "Manage my loans",
            "Sign Out",
            "Return to the Bank Selection"
        },
        "\t\tChoose the right point:"
    )
    {
        ChosenBank = MainView.BankM!.ChosenBank;
        Session = MainView.AuthM[ChosenBank]!.Session;

        Action.Add(Controls[0], ShowInfo);
        Action.Add(Controls[1], ManageAccs);
        Action.Add(Controls[2], ManageLoans);
        Action.Add(Controls[3], SignOut);
        Action.Add(Controls[4], Exit);
    }

    private AccountMenu? AccM { get; set; }
    private LoanMenu? LoanM { get; set; }

    private protected void ManageLoans()
    {
        LoanM = new LoanMenu();
        MenuController.Print(LoanM);
        LoanM.Operate();
    }

    private protected void ManageAccs()
    {
        AccM = new AccountMenu();
        MenuController.Print(AccM);
        AccM.Operate();
    }

    private protected void ShowInfo()
    {
        Console.Clear();
        Console.WriteLine("\t\tClient Info:");

        Console.WriteLine(Session);

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private void SignOut()
    {
        Ext = true;
        Session = null;
        AccM = null;
        MainView.AuthM[ChosenBank]!.SignOut();

        MenuController.Print(MainView.BankM);
    }

    protected void Exit()
    {
        Ext = true;
        MainView.AuthM[ChosenBank]!.Exit();
    }

    public void Operate()
    {
        while (!Ext)
        {
            var ch = MenuController.MenuChoice(Controls.Count);
            switch (ch)
            {
                case var n and > 0 when n <= Controls.Count:
                    Action[Controls[ch - 1]]();

                    break;
            }
        }
    }
}