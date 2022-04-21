using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class AuthMenu : Menu
{
    public AuthMenu() : base
    (
        new List<string>
        {
            "Sign In",
            "Client Sign Up",
            "Business Sign Up",
            "Go Back"
        },
        $"\t\t\tBank Info:\n{BankController.ShowInfo()}" +
        "\t\tChoose the right point:"
    )
    {
        ChosenBank = MainView.BankM!.ChosenBank;

        Action.Add(Controls[0], SignIn);
        Action.Add(Controls[1], ClientSignUp);
        Action.Add(Controls[2], BusinessSignUp);
        Action.Add(Controls[3], Exit);
    }

    private void SignIn()
    {
        Console.Clear();
        while (Session == null)
        {
            Console.WriteLine("\t\tFill up the required data:");
            Console.Write("• Enter your login: ");
            var login = Console.ReadLine();
            Console.Write("• Enter your password: ");
            var pass = Console.ReadLine();

            Session = MainController.SignIn(login!, pass!, ChosenBank + 1);

            if (Session != null) break;

            MenuController.Clear(2);
            Console.WriteLine("⚠ Error! False data!\n1. ⁍ Try Again\n2. ⦻ Return");

            var ch = MenuController.MenuChoice(2);

            switch (ch)
            {
                case 1:
                    Console.Clear();

                    break;

                case 2:
                    MenuController.Print(this);

                    return;
            }
        }

        if (Session != null)
        {
            MainView.ClientM[ChosenBank] = Session.GetType().Name switch
            {
                "Client" => new ClientMenu(),
                "Company" => new CompanyMenu(),
                _ => new StaffMenu()
            };

            MenuController.Print(MainView.ClientM[ChosenBank]);
            MainView.ClientM[ChosenBank]!.Operate();
            Ext = true;
        }
        else
        {
            MenuController.Print(this);
        }
    }

    private void ClientSignUp()
    {
        Console.Clear();
        Console.WriteLine("\t\tFill up the required data:");

        var lData = LogInfo();
        var uData = ClientInfo();

        MainController.ClientSignUp(lData, uData);

        Console.WriteLine("\t  Thank you for the registration!");
        Console.WriteLine("⌛ Please wait till our managers confirm your registration request.");
        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private void BusinessSignUp()
    {
        Console.Clear();
        Console.WriteLine("\t\tFill up the required data:");

        var lData = LogInfo();
        var bData = BusinessInfo();

        MainController.ComSignUp(lData, bData);

        Console.WriteLine("\t  Thank you for the registration!");
        Console.WriteLine("⌛ Please wait till our managers confirm your registration request.");
        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private List<string> LogInfo()
    {
        Console.Write("• Enter preferred username: ");

        var login = DataController.CheckLogin(Console.ReadLine(), ChosenBank + 1);
        string pass;

        while (true)
        {
            Console.Write("• Enter your password: ");
            pass = DataController.CheckPassword(Console.ReadLine());
            Console.Write("• Repeat your password: ");
            var pass2 = Console.ReadLine();

            if (pass == pass2)
            {
                Console.Clear();
                Console.WriteLine("\t\tFill up the required data:");

                break;
            }

            Console.Clear();
            Console.WriteLine("\t\tFill up the required data:");
            Console.WriteLine("✘ Passwords don't match! Try again!");
        }

        return new List<string> {login, pass, (ChosenBank + 1).ToString()};
    }

    private static List<string> ClientInfo()
    {
        var uData = new List<string>();

        Console.Write("• Enter your Name: ");
        uData.Add(DataController.CheckName(Console.ReadLine()));
        Console.Write("• Enter your Passport No': ");
        uData.Add(DataController.CheckPassNum(Console.ReadLine()));
        Console.Write("• Enter your Passport ID: ");
        uData.Add(DataController.CheckPassId(Console.ReadLine()));
        Console.Write("• Enter your Phone: ");
        uData.Add(DataController.CheckPhone(Console.ReadLine()));
        Console.Write("• Enter your Email: ");
        uData.Add(DataController.CheckEmail(Console.ReadLine()));

        return uData;
    }

    private static List<string> BusinessInfo()
    {
        var bData = new List<string>();

        Console.Write("• Enter your Company legal name: ");
        bData.Add(DataController.CheckComName(Console.ReadLine()));
        Console.Write("• Enter your Company type (ZAO, OAO, OOO, IP): ");
        bData.Add(DataController.CheckComType(Console.ReadLine()));
        Console.Write("• Enter your Company legal address: ");
        bData.Add(DataController.CheckComAddress(Console.ReadLine()));
        Console.Write("• Enter your Company UNP: ");
        bData.Add(DataController.CheckUnp(Console.ReadLine()));
        Console.Write("• Enter your Company BIC: ");
        bData.Add(DataController.CheckBic(Console.ReadLine()));

        return bData;
    }

    public void SignOut()
    {
        Ext = true;
        Session = null;
    }

    public void Exit()
    {
        Ext = true;
        MenuController.Print(MainView.BankM);
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

        Ext = false;
    }
}