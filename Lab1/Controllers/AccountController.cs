using Lab1.Models;

namespace Lab1.Controllers;

public static class AccountController
{
    public static async Task<List<Account>?> Refresh(int id, int bankId)
    {
        return await MainController.Db!.GetAccounts(id, bankId);
    }

    public static int ChooseAcc(List<Account> accounts)
    {
        for (var i = 0; i < accounts.Count; i++) Console.Write($"{i + 1}. " + accounts[i]);

        Console.WriteLine($"{accounts.Count + 1}. ⦻ Go Back");
        var choice = MenuController.MenuChoice(accounts.Count + 1);
        MenuController.Clear(accounts.Count + 2);

        return choice;
    }

    public static async Task AddAcc(Account acc)
    {
        await MainController.Db!.AddAcc(acc);
    }

    public static async Task RemoveAcc(Account acc)
    {
        await MainController.Db!.RemoveAcc(acc);
    }

    public static async Task ReplenishAcc(Account acc, string sum)
    {
        await MainController.Db!.ReplenishAcc(acc.Id, sum);
    }

    public static async Task<bool> CashOutAcc(int id, string sum)
    {
        return await MainController.Db!.CashOutAcc(id, sum);
    }

    public static async Task<bool> AccCurrencyCheck(AccountTransfer transfer)
    {
        return await MainController.Db!.AccCurrencyCheck(transfer);
    }

    public static async Task<bool> AccTransfer(AccountTransfer transfer)
    {
        return await MainController.Db!.AccTransfer(transfer);
    }

    public static async Task<Account?> CheckAccId(int id)
    {
        return await MainController.Db!.CheckAccId(id);
    }
}