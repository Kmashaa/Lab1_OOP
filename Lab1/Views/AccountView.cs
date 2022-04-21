using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class AccountMenu : Menu
{
    protected new readonly Dictionary<string, Choice> Action = new();

    public AccountMenu() : base
    (
        new List<string>
        {
            "Show my accounts",
            "Open new account",
            "Replenish my account",
            "Cash Out my account",
            "Transfer funds to another account",
            "Close your account",
            "Go Back"
        },
        "\t\tChoose the right point:"
    )
    {
        ChosenBank = MainView.BankM!.ChosenBank;
        Session = MainView.AuthM[ChosenBank]!.Session;

        Action.Add(Controls[0], ShowAccounts);
        Action.Add(Controls[1], AccAdd);
        Action.Add(Controls[2], ReplenishAcc);
        Action.Add(Controls[3], CashOutAcc);
        Action.Add(Controls[4], AccTransfer);
        Action.Add(Controls[5], AccRemove);
        Action.Add(Controls[6], Exit);
    }

    private List<Account>? Accounts { get; set; }

    private Task Exit()
    {
        Ext = true;
        MenuController.Print(MainView.ClientM[ChosenBank]);

        return Task.CompletedTask;
    }

    private Task ShowAccounts()
    {
        Console.Clear();
        Console.WriteLine("\t\tYour active accounts list:");

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;

        if (Accounts?.Capacity > 0)
            foreach (var acc in Accounts)
                Console.Write("⊢  ¤ " + acc);
        else
            Console.WriteLine("⚠ Sadly, you have not any opened accounts.");

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);

        return Task.CompletedTask;
    }

    private async Task AccAdd()
    {
        Console.Clear();

        Console.WriteLine("\t\tFill up the required data:");
        Console.WriteLine("• Select the currency of the new account");

        char[] currency = {'$', '€', '₽', '£'};
        for (var i = 0; i < currency.Length; i++) Console.WriteLine($"{i + 1}. {currency[i]}");
        var ch = MenuController.MenuChoice(5);
        MenuController.Clear(5);

        Console.Write("• Enter the sum of money: ");
        var money = DataController.CheckMoney(Console.ReadLine());

        await AccountController.AddAcc(new Account(new List<string>
        {
            "-1", ChosenBank.ToString(),
            Session!.GetInfo()![0], money, currency[ch - 1].ToString()
        }));
        Console.Write("✔ Account was successfully opened");

        Console.WriteLine("\n1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        MenuController.Print(this);
    }

    private async Task AccRemove()
    {
        Console.Clear();

        Console.WriteLine("\t\tFill up the required data:");
        Console.WriteLine("• Choose account to close:");

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        if (Accounts!.Capacity > 0)
        {
            var ch = AccountController.ChooseAcc(Accounts);

            if (ch == Accounts.Count + 1)
            {
                MenuController.Print(this);

                return;
            }

            await AccountController.RemoveAcc(Accounts[ch - 1]);
            Console.WriteLine("✔ Account was successfully closed");
        }
        else
        {
            Console.WriteLine("⚠ Sadly, you have no active accounts to close.");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        MenuController.Print(this);
    }

    private async Task ReplenishAcc()
    {
        Console.Clear();

        Console.WriteLine("\t\tFill up the required data:");
        Console.WriteLine("• Choose account to replenish:");

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        if (Accounts!.Capacity > 0)
        {
            var ch = AccountController.ChooseAcc(Accounts);

            if (ch == Accounts.Count + 1)
            {
                MenuController.Print(this);

                return;
            }

            Console.Write("• Enter the sum of money: ");
            var money = DataController.CheckMoney(Console.ReadLine());

            await AccountController.ReplenishAcc(Accounts[ch - 1], money);
            Console.WriteLine("✔ Account was successfully replenished");
        }
        else
        {
            Console.WriteLine("⚠ Sadly, you have no active accounts to replenish.");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        MenuController.Print(this);
    }

    private Task CashOutAcc()
    {
        Console.Clear();

        Console.WriteLine("\t\tFill up the required data:");
        Console.WriteLine("• Choose account to cash out:");

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        if (Accounts!.Capacity > 0)
        {
            var ch = AccountController.ChooseAcc(Accounts);

            if (ch == Accounts.Count + 1)
            {
                MenuController.Print(this);

                return Task.CompletedTask;
            }

            Console.Write("• Enter the sum of money: ");
            var money = DataController.CheckMoney(Console.ReadLine());

            Console.WriteLine(AccountController.CashOutAcc(Accounts[ch - 1].Id, money).Result
                ? "✔ Account was successfully cashed out"
                : "✘ Account has insufficient funds");
        }
        else
        {
            Console.WriteLine("⚠ Sadly, you have no active accounts to cash out.");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        MenuController.Print(this);

        return Task.CompletedTask;
    }

    private Task AccTransfer()
    {
        Console.Clear();

        Console.WriteLine("\t\tFill up the required data:");
        Console.WriteLine("• Choose account to transfer from:");

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        if (Accounts!.Capacity > 0)
        {
            var ch = AccountController.ChooseAcc(Accounts);

            if (ch == Accounts.Count + 1)
            {
                MenuController.Print(this);

                return Task.CompletedTask;
            }

            Console.Write("• Enter beneficiary's account id: ");
            var id = Convert.ToInt32(DataController.CheckAccId(Console.ReadLine()));
            Console.Write("• Enter the sum of money: ");
            var money = DataController.CheckMoney(Console.ReadLine());

            if (id != Accounts[ch - 1].Id)
            {
                var getter = AccountController.CheckAccId(id).Result;

                if (getter != null)
                {
                    var tmp = new AccountTransfer(new List<Account> {Accounts[ch - 1], getter}, money);

                    if (AccountController.AccCurrencyCheck(tmp).Result)
                        Console.WriteLine(AccountController.AccTransfer(tmp).Result
                            ? "✔ Transfer Successful"
                            : "✘ Transfer error: insufficient funds");
                    else
                        Console.WriteLine("✘ Transfer error: currency mismatch");
                }
                else
                {
                    Console.WriteLine("✘ Transfer error: invalid id");
                }
            }
            else
            {
                Console.WriteLine("You can't transfer to this account");
            }
        }
        else
        {
            Console.WriteLine("⚠ Sadly, you have no active accounts to proceed transfers.");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        Accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        MenuController.Print(this);

        return Task.CompletedTask;
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

    protected new delegate Task Choice();
}