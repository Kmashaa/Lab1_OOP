using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class LoanMenu : Menu
{
    private List<double> _loanRates;

    public LoanMenu() : base
    (
        new List<string>
        {
            "Show my active loans",
            "Request for a new loan",
            "Pay off the monthly payment",
            "Go Back"
        },
        "\t\tChoose the right point:"
    )
    {
        ChosenBank = MainView.BankM!.ChosenBank;
        Session = MainView.AuthM[ChosenBank]!.Session;

        Action.Add(Controls[0], RefreshLoans);
        Action.Add(Controls[1], NewLoan);
        Action.Add(Controls[2], PayOff);
        Action.Add(Controls[3], Exit);

        _loanRates = new List<double>();
    }

    private List<Loan>? Loans { get; set; }

    private void RefreshLoans()
    {
        Console.Clear();
        Console.WriteLine("\t\t\tActive loans:");

        Loans = LoanController.Refresh(Convert.ToInt32(Session!.GetInfo()![0])).Result;
        if (Loans != null)
            foreach (var ln in Loans)
                Console.Write("⊢  " + ln);
        else
            Console.WriteLine("• Currently you have not any active loans");

        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private async void NewLoan()
    {
        Console.Clear();
        Console.WriteLine("\t\tFill up the required data:");

        if (!ChooseLoanType())
        {
            MenuController.Print(this);

            return;
        }

        Console.WriteLine("• Choose account to proceed loan request");

        var accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
        if (accounts!.Capacity > 0)
        {
            var ch = AccountController.ChooseAcc(accounts);

            if (ch == accounts.Count + 1)
            {
                MenuController.Print(this);

                return;
            }

            var loanInfo = LoanInfo();

            await LoanController.RequestLoan(new Loan(
                new List<string>
                {
                    (-1).ToString(), Session!.GetInfo()![0], accounts[ch - 1].Id.ToString(),
                    loanInfo[0], loanInfo[1], loanInfo[2], loanInfo[3], loanInfo[4], loanInfo[5], loanInfo[6]
                }), ChosenBank + 1);

            Console.WriteLine("✔ Loan Request was successfully created.");
            Console.WriteLine("⌛ Please wait till our managers confirm your loan request.");
        }
        else
        {
            Console.WriteLine("⚠ Sadly, you have no active accounts to proceed the salary request.");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private List<string> LoanInfo()
    {
        Console.Write("• Enter the sum of the preferred loan: ");
        var sum = DataController.CheckMoney(Console.ReadLine());

        Console.Write("• Select the time of the preferred loan:" +
                      "\n1. 3 month\n2. 6 month\n3. 12 month\n4. 24 month\n5. > 24 month");
        var ch = MenuController.MenuChoice(5);
        MenuController.Clear(5);

        string loanT;
        if (ch == 5)
        {
            Console.Write("• Enter the loan time (must be / 6): ");
            loanT = DataController.CheckTime(Console.ReadLine());
        }
        else
        {
            loanT = ch switch
            {
                1 => 3.ToString(),
                2 => 6.ToString(),
                3 => 12.ToString(),
                4 => 24.ToString(),
                _ => throw new ArgumentOutOfRangeException()
            };
            Console.WriteLine($"✔ Selected loan time: {loanT}");
        }

        var monPay = Convert.ToDouble(sum) * _loanRates[ch - 1];
        monPay *= Convert.ToDouble(loanT) / 12;
        monPay += Convert.ToDouble(sum);

        var date = DateTime.Now.ToString("dd.MM.yyyy");

        return new List<string>
        {
            sum, monPay.ToString("0.##"), loanT,
            (monPay / Convert.ToDouble(loanT)).ToString("0.##"), "0", "0", date
        };
    }

    private bool ChooseLoanType()
    {
        Console.Write("• Select the type of the preferred loan:" +
                      "\n1. Simple Loan (Higher %)\n2. Default Installment (Lower %)\n3. ⦻ Go Back");

        var ch = MenuController.MenuChoice(3);
        switch (ch)
        {
            case 1:
                _loanRates = new List<double> {0.245, 0.121, 0.20, 0.15, 0.085};
                MenuController.Clear(3);

                return true;

            case 2:
                _loanRates = new List<double> {0.045, 0.092, 0.12, 0.15, 0.185};
                MenuController.Clear(3);

                return true;

            default:

                return false;
        }
    }

    private async void PayOff()
    {
        Console.Clear();
        LoanController.UpdateDebts(Convert.ToInt32(Session!.GetInfo()![0]));
        var loans = await MainController.Db!.GetLoans(Convert.ToInt32(Session!.GetInfo()![0]));

        Console.WriteLine("\t\tChoose loan to proceed month pay off");
        if (loans != null)
        {
            var dLoans = 0;

            var sortLoans = loans.Where(t => Convert.ToDouble(t.Debt) > 0).ToList();
            foreach (var t in sortLoans)
                Console.WriteLine($"{++dLoans}. Loan Sum: {t.Sum,-10} |   Loan Debt: {t.Debt}");

            if (dLoans == 0) Console.WriteLine("• Currently you have no debts");
            Console.WriteLine($"{dLoans + 1}. ⦻ Go Back");
            var ch = MenuController.MenuChoice(dLoans + 1);

            if (ch == dLoans + 1)
            {
                MenuController.Print(this);

                return;
            }

            MenuController.Clear(dLoans + 1);
            Console.WriteLine(LoanController.PayOffLoan(sortLoans[ch - 1]).Result
                ? "✔ Loan month debt was successfully payed off"
                : "✘ Error in processing: loan account has insufficient funds");
        }
        else
        {
            Console.WriteLine("• Currently you have not any active loans");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private void Exit()
    {
        Ext = true;
        MenuController.Print(MainView.ClientM[MainView.BankM!.ChosenBank]);
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