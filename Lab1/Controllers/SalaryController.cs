namespace Lab1.Controllers;

public static class SalaryController
{
    public static async Task<Dictionary<int, string>?> RefreshWorkers(int id)
    {
        return await MainController.Db!.GetCompWorkers(id);
    }

    public static async void SetWorkers(int id, Dictionary<int, string> workers)
    {
        await MainController.Db!.SetCompWorkers(id, workers);
    }

    public static async void GenSalaryRequest(List<int> ids, string sum)
    {
        await MainController.Db!.GenSalaryRequest(ids, sum);
    }
}