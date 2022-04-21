using Lab1.Controllers;

namespace Lab1.Views;

public class CompanyMenu : ClientMenu
{
    public CompanyMenu()
    {
        Controls = new List<string>
        {
            "Show the company's info",
            "Manage company's accounts",
            "Manage company's loans",
            "Perform the salary request",
            "Sign Out",
            "Return to the Bank Selection"
        };
        Action.Clear();

        Action.Add(Controls[0], ShowInfo);
        Action.Add(Controls[1], ManageAccs);
        Action.Add(Controls[2], ManageLoans);
        Action.Add(Controls[3], SalaryRequest);
        Action.Add(Controls[4], SignOut);
        Action.Add(Controls[5], Exit);
    }

    private Dictionary<int, string>? Workers { get; set; }

    private void SignOut()
    {
        Ext = true;
        Session = null;
        MainView.AuthM[ChosenBank]!.SignOut();

        MenuController.Print(MainView.BankM);
    }

    private async void SalaryRequest()
    {
        Console.Clear();
        Console.WriteLine("\t\tSalary Request");
        Workers = SalaryController.RefreshWorkers(Convert.ToInt32(Session!.GetInfo()![0])).Result;

        if (Workers == null)
        {
            Console.WriteLine("⚠ Sadly workers have not been added yet\n1. ⁍ Add workers now\n2. ⦻ Return");
            var ch = MenuController.MenuChoice(2);

            if (ch == 1) AddWorkers();
        }
        else
        {
            var accounts = AccountController.Refresh(Convert.ToInt32(Session!.GetInfo()![0]), ChosenBank + 1).Result;
            if (accounts != null)
            {
                Console.WriteLine("• Choose account to proceed salary request");
                var ch = AccountController.ChooseAcc(accounts);

                if (ch == accounts.Count + 1)
                {
                    MenuController.Print(this);

                    return;
                }

                var sum = Workers.Sum(worker => Convert.ToDouble(worker.Value));

                if (AccountController.CashOutAcc(accounts[ch - 1].Id, sum.ToString("0.##")).Result == false)
                {
                    Console.WriteLine("✘ Error in proceeding request! Insufficient funds on account");
                }
                else
                {
                    await AccountController.ReplenishAcc(accounts[ch - 1], sum.ToString("0.##"));

                    SalaryController.GenSalaryRequest(new List<int>
                    {
                        Convert.ToInt32(Session!.GetInfo()![0]),
                        accounts[ch - 1].Id, ChosenBank + 1
                    }, sum.ToString("0.##"));

                    Console.WriteLine("✔ Salary Request was successfully created.");
                    Console.WriteLine("⌛ Please wait till our managers confirm your salary request.");
                }
            }
            else
            {
                Console.WriteLine("⚠ Sadly, you have no active accounts to proceed the salary request.");
            }
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private void AddWorkers()
    {
        MenuController.Clear(3);

        Workers = new Dictionary<int, string>();
        while (true)
        {
            Console.Write("• Enter worker's account id: ");
            var id = Convert.ToInt32(DataController.CheckWorkerId(Console.ReadLine()));
            Console.Write("• Enter worker's salary: ");
            var money = DataController.CheckMoney(Console.ReadLine());
            Workers!.Add(id, money);

            Console.WriteLine("1. • Add one more\n2. ⦻ Return");

            if (MenuController.MenuChoice(2) == 2) break;
            MenuController.Clear(4);
        }

        SalaryController.SetWorkers(Convert.ToInt32(Session!.GetInfo()![0]), Workers);
    }
}