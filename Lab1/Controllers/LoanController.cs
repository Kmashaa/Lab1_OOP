using Lab1.Models;

namespace Lab1.Controllers;

public static class LoanController
{
    public static async Task<List<Loan>?> Refresh(int userId)
    {
        return await MainController.Db!.GetLoans(userId);
    }

    public static async Task RequestLoan(Loan newLoan, int bankId)
    {
        await MainController.Db!.RequestLoan(newLoan, bankId);
    }

    public static async void UpdateDebts(int userId)
    {
        var loans = await MainController.Db!.GetLoans(userId);
        if (loans == null) return;

        foreach (var t in loans)
        {
            var day = Convert.ToInt32(DateTime.Now.Day) + Convert.ToInt32(DateTime.Now.Month) * 30;
            var tDay = Convert.ToInt32(t.Date.Substring(3, 2)) * 30;
            tDay += Convert.ToInt32(t.Date[..2]);

            if (day - tDay == 0) continue;
            double result = day - tDay - t.Payments;

            await MainController.Db.UpdateDebt(t.Id, result * Convert.ToDouble(t.MonPayment));
        }
    }

    public static async Task<bool> PayOffLoan(Loan loan)
    {
        return await MainController.Db!.PayOffLoan(loan);
    }
}