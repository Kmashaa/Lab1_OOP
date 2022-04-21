using Lab1.Controllers;
using Lab1.Models;

namespace Lab1.Views;

public class StaffMenu : ClientMenu
{
    public StaffMenu()
    {
        Controls = new List<string>
        {
            "Check current clients money flows",
            "Revert last account action",
            "Block / Freeze / Activate account",
            "Check new registrations' requests",
            "Check salary requests",
            "Check loans requests",
            "Check general logs",
            "Sign Out",
            "Return to the Bank Selection"
        };

        Action.Clear();
        Action.Add(Controls[0], MoneyFlows);
        Action.Add(Controls[1], RevAccAct);
        Action.Add(Controls[2], BlFrAcc);
        Action.Add(Controls[3], CheckAccReq);
        Action.Add(Controls[4], CheckSalReq);
        Action.Add(Controls[5], CheckLoansReq);
        Action.Add(Controls[6], CheckLogs);
        Action.Add(Controls[7], SignOut);
        Action.Add(Controls[8], Exit);
    }

    private async void BlFrAcc()
    {
        Console.Clear();
        Console.WriteLine("\t\tFill up the required data:");

        if (Convert.ToInt32(Session!.GetInfo()![1]) > 3)
        {
            var choice = AccBlInfo();

            Console.Write("• Select account action:\n1. Block\n2. Freeze\n3. Activate");
            var ch = MenuController.MenuChoice(3);

            switch (ch)
            {
                case 1:
                    await StaffController.ChangeState(choice[1], Convert.ToInt32(choice[0]), "blocked");

                    break;

                case 2:
                    await StaffController.ChangeState(choice[1], Convert.ToInt32(choice[0]), "frozen");

                    break;

                case 3:
                    await StaffController.ChangeState(choice[1], Convert.ToInt32(choice[0]), "active");

                    break;
            }

            MenuController.Clear(4);
            Console.WriteLine("✔ Action was successfully applied");
        }
        else
        {
            Console.WriteLine("⚠ Sorry, it seems you have no right to use it");
        }

        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private static List<string> AccBlInfo()
    {
        Console.WriteLine("• Select the aim:\n1. User account\n2. Bank account");
        var ch = MenuController.MenuChoice(2);
        MenuController.Clear(3);

        Console.Write("• Enter account id: ");
        string id = ch switch
        {
            1 => DataController.CheckUserId(Console.ReadLine()).Result,
            _ => DataController.CheckWorkerId(Console.ReadLine()),
        };
        var where = ch == 1 ? "clients" : "accounts";

        return new List<string> {id, where};
    }

    private void MoneyFlows()
    {
        Console.Clear();
        Console.WriteLine("\t\t\t\tBank client's money flows");

        if (Convert.ToInt32(Session!.GetInfo()![1]) > 1)
        {
            var mFlows = StaffController.CheckMoneyFlow(ChosenBank + 1).Result;
            if (mFlows.Capacity > 0)
                foreach (var flow in mFlows)
                    Console.Write("⊢  " + flow);
            else
                Console.WriteLine("⚠ Sadly, there is no money transfers in this bank.");
        }
        else
        {
            Console.WriteLine("⚠ Sorry, it seems you have no right to use it");
        }

        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private void RevAccAct()
    {
        Console.Clear();

        if (Convert.ToInt32(Session!.GetInfo()![1]) > 1)
        {
            Console.WriteLine("\t\tFill up the required data:");

            Console.Write("• Enter account id: ");
            var id = Convert.ToInt32(DataController.CheckWorkerId(Console.ReadLine()));
            var transfers = StaffController.GetTransfers(ChosenBank + 1, id).Result;

            Revert(transfers.Last());
        }
        else
        {
            Console.WriteLine("⚠ Sorry, it seems you have no right to use it");
        }

        Console.WriteLine("1. ⦻ Go Back");

        MenuController.MenuChoice(1);
        MenuController.Print(this);
    }

    private static async void Revert(AccountTransfer accTrans)
    {
        if (accTrans.GetId != -1)
        {
            if (accTrans.SendId == -1)
            {
                await AccountController.CashOutAcc(accTrans.GetId, accTrans.Sum);
            }
            else
            {
                (accTrans.GetId, accTrans.SendId) = (accTrans.SendId, accTrans.GetId);
                (accTrans.BankGetId, accTrans.BankSendId) = (accTrans.BankSendId, accTrans.BankGetId);

                await AccountController.AccTransfer(accTrans);
            }

            Console.WriteLine("✔ Last account action was successfully reverted");
        }
        else
        {
            Console.WriteLine("✘ Error, debit can not be reverted");
        }
    }

    private void CheckAccReq()
    {
        CheckReq("clients", 2);
    }

    private void CheckSalReq()
    {
        CheckReq("salary_requests", 1);
    }

    private void CheckLoansReq()
    {
        CheckReq("loans_requests", 2);
    }

    private void CheckReq(string where, int accessLvl)
    {
        Console.Clear();
        Console.WriteLine("\t\tChoose awaiting request:");

        if (Convert.ToInt32(Session!.GetInfo()![1]) > accessLvl)
        {
            var reqs = StaffController.GetReqData(where, ChosenBank + 1).Result;
            if (reqs != null)
            {
                Console.WriteLine("• Choose awaiting request to operate:");
                for (var i = 0; i < (reqs.Count < 8 ? reqs.Count : 8); i++)
                    switch (where)
                    {
                        case "salary_requests":
                            Console.WriteLine($"{i + 1}. ⊢  Req. ID: {reqs[i][0],-2} | " +
                                              $"Company ID: {reqs[i][1],-5} | " +
                                              $"Acc ID: {reqs[i][2],-2} | " +
                                              $"Total Sum: {reqs[i][3],-5}");
                            break;

                        case "loans_requests":
                            Console.WriteLine($"{i + 1}. ⊢  Req. ID: {reqs[i][0],-2} | " +
                                              $"Client ID: {reqs[i][1],-5} | " +
                                              $"Acc ID: {reqs[i][2],-2} | " +
                                              $"Loan Sum: {reqs[i][3],-5}");
                            break;

                        case "clients":
                            Console.WriteLine($"{i + 1}. ⊢  Client ID: {reqs[i][0],-2} | " +
                                              $"Login: {reqs[i][1],-10} | " +
                                              $"Password: {reqs[i][2],-10} | " +
                                              $"Role: {reqs[i][3],-3}");
                            break;
                    }

                Console.WriteLine($"{(reqs.Count < 8 ? reqs.Count : 8) + 1}. ⦻ Go Back");
                var ch = MenuController.MenuChoice((reqs.Count < 8 ? reqs.Count : 8) + 1);

                if (ch == (reqs.Count < 8 ? reqs.Count : 8) + 1)
                {
                    MenuController.Print(this);

                    return;
                }

                MenuController.Clear((reqs.Count < 8 ? reqs.Count : 8) + 2);
                ChangeReqSt(where, reqs[ch - 1]);
            }
            else
            {
                Console.WriteLine("⚠ Sadly, there is no awaiting requests right now");
            }
        }
        else
        {
            Console.WriteLine("⚠ Sorry, it seems you have no rights to use it");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private static async void ChangeReqSt(string where, List<string> data)
    {
        Console.WriteLine("• Choose the action to do:");
        Console.WriteLine("1. Approve\n2. Decline");
        var ch = MenuController.MenuChoice(2);

        switch (ch)
        {
            case 1:
                switch (where)
                {
                    case "salary_requests":
                        await StaffController.ApproveSalReq(data);
                        break;

                    case "loans_requests":
                        await AccountController.ReplenishAcc((
                            await AccountController.CheckAccId(Convert.ToInt32(data[2])))!, data[3]);
                        break;
                }

                await StaffController.ChangeState(where,
                    Convert.ToInt32(data[0]), "approved");

                MenuController.Clear(3);
                Console.WriteLine("✔ Request was successfully approved");

                break;

            case 2:
                await StaffController.ChangeState(where,
                    Convert.ToInt32(data[0]), "declined");

                MenuController.Clear(3);
                Console.WriteLine("✔ Request was successfully declined");

                break;
        }
    }

    private void CheckLogs()
    {
        Console.Clear();
        Console.WriteLine("\t\tChoose awaiting request:");

        if (Convert.ToInt32(Session!.GetInfo()![1]) > 3)
        {
            var logs = StaffController.GetLogs().Result;
            if (logs != null)
                foreach (var log in logs)
                    Console.WriteLine($"⊢  Log Type: {log[0],-25} | Affected ID: {log[1],-3}");
            else
                Console.WriteLine("⚠ Sadly, there is no logs right now");
        }
        else
        {
            Console.WriteLine("⚠ Sorry, it seems you have no rights to use it");
        }

        Console.WriteLine("1. ⦻ Go Back");
        MenuController.MenuChoice(1);

        MenuController.Print(this);
    }

    private void SignOut()
    {
        Ext = true;
        Session = null;
        MainView.AuthM[ChosenBank]!.SignOut();

        MenuController.Print(MainView.BankM);
    }
}