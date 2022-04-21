using Lab1.Models;

namespace Lab1.Controllers;

public class StaffController
{
    public static async Task<List<AccountTransfer>> CheckMoneyFlow(int bankId)
    {
        return await MainController.Db!.GetTransfers(bankId);
    }

    public static async Task<List<AccountTransfer>> GetTransfers(int bankId, int accId)
    {
        return await MainController.Db!.GetTransfers(bankId, accId);
    }

    public static async Task<List<List<string>>?> GetReqData(string where, int bankId)
    {
        return await MainController.Db!.GetReqs(where, bankId);
    }

    public static async Task ApproveSalReq(List<string> data)
    {
        var workers = await MainController.Db!.GetCompWorkers(Convert.ToInt32(data[1]));

        foreach (var (id, salary) in workers!)
        {
            var transfer = new AccountTransfer(
                new List<Account>
                {
                    (await MainController.Db.CheckAccId(Convert.ToInt32(data[2])))!,
                    (await MainController.Db.CheckAccId(id))!
                }, salary);

            await MainController.Db.AccTransfer(transfer);
        }
    }

    public static async Task ChangeState(string where, int id, string status)
    {
        await MainController.Db!.ChangeStatus(where, id, status);
    }

    public static async Task<List<List<string>>?> GetLogs()
    {
        return await MainController.Db!.GetLogs();
    }
}