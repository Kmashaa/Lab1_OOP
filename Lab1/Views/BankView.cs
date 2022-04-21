using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class BankChoiceMenu : Menu
{
    public BankChoiceMenu() : base
    (
        MainController.Db!.BankList().Result,
        "\t\tChoose the bank system:"
    )
    {
        var i = 0;
        for (; i < Controls.Count - 1; i++) Action.Add(Controls[i], BankChoice);

        Action.Add(Controls[i], Exit);
    }

    private void BankChoice()
    {
        Console.Clear();
        if (MainView.AuthM[ChosenBank]?.Session == null)
        {
            MainView.AuthM[ChosenBank] = new AuthMenu();
            MenuController.Print(MainView.AuthM[ChosenBank]);
            MainView.AuthM[ChosenBank]!.Operate();
        }
        else
        {
            MainView.ClientM[ChosenBank] = MainView.AuthM[ChosenBank]!.Session!.GetType().Name switch
            {
                "Client" => new ClientMenu(),
                "Company" => new CompanyMenu(),
                _ => new StaffMenu()
            };

            MenuController.Print(MainView.ClientM[ChosenBank]);
            MainView.ClientM[ChosenBank]!.Operate();
        }
    }

    private void Exit()
    {
        Ext = true;
    }

    public void Operate()
    {
        while (!Ext)
        {
            var ch = MenuController.MenuChoice(Controls.Count);
            switch (ch)
            {
                case var n and > 0 when n <= Controls.Count:
                    if (ch != 4)
                        ChosenBank = ch - 1;

                    Action[Controls[ch - 1]]();

                    break;
            }
        }
    }
}