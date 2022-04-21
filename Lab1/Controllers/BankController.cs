using Lab1.Views;

namespace Lab1.Controllers;

public static class BankController
{
    public static string ShowInfo()
    {
        var bank = MainView.Banks![MainView.BankM!.ChosenBank];
        return $"{"• Bank Type:",-25}{bank.Type,-18}\n" +
               $"{"• Bank Name:",-25}{bank.Name}\n" +
               $"{"• Bank legal address:",-25}{bank.Address,-18}\n" +
               $"{"• Bank UNP:",-25}{bank.Unp,-18}\n" +
               $"{"• Bank BIC:",-25}{bank.Bic,-18}\n";
    }
}