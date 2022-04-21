using Lab1.Models;

namespace Lab1.Controllers;

public static class MainController
{
    public static DataBase? Db { get; private set; }

    public static async Task<List<Bank>?> Initialize()
    {
        Db = await DataBase.CreateAsync("secretKey");

        return Db.BankInfo().Result;
    }

    public static User? SignIn(string login, string pass, int bank)
    {
        return Db!.ClientSignIn(login, pass, bank).Result;
    }

    public static async void ClientSignUp(List<string> lData, List<string> uData)
    {
        await Db!.ClientSignUp(lData, uData);
    }

    public static async void ComSignUp(List<string> lData, List<string> bData)
    {
        await Db!.ComSignUp(lData, bData);
    }
}