using System.Text.RegularExpressions;

namespace Lab1.Controllers;

public static class DataController
{
    public static string CheckLogin(string? login, int bank)
    {
        while (MainController.Db!.CheckLogin(login, bank).Result)
        {
            MenuController.Clear(1);
            Console.Write("✘ This username is taken, try another one: ");
            login = Console.ReadLine();
        }

        while (!Regex.IsMatch(login ?? string.Empty, "^[a-zA-Z][a-zA-Z0-9]{3,9}$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect username format, try again please: ");
            login = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Username: {login}");

        return login!;
    }

    public static string CheckPassword(string? pass)
    {
        while (pass == string.Empty) pass = TryAgain("✘ Password must not be empty.");

        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMiniMaxChars = new Regex(@".{7,10}");
        var hasLowerChar = new Regex(@"[a-z]+");

        while (true)
        {
            if (!hasLowerChar.IsMatch(pass ?? string.Empty))
            {
                Console.WriteLine(" ");
                pass = TryAgain("✘ Password must contain At least one lower case letter.");

                continue;
            }

            if (!hasUpperChar.IsMatch(pass ?? string.Empty))
            {
                Console.WriteLine(" ");
                pass = TryAgain("✘ Password must contain At least one upper case letter.");

                continue;
            }

            if (!hasMiniMaxChars.IsMatch(pass ?? string.Empty))
            {
                Console.WriteLine(" ");
                pass = TryAgain("✘ Password must not be less than 8 characters.");

                continue;
            }

            if (!hasNumber.IsMatch(pass ?? string.Empty))
            {
                Console.WriteLine(" ");
                pass = TryAgain("✘ Password must contain At least one numeric value.");

                continue;
            }

            break;
        }

        return pass!;
    }

    private static string? TryAgain(string mes)
    {
        MenuController.Clear(2);
        Console.WriteLine(mes);
        Console.Write("• Try again, please: ");

        return Console.ReadLine();
    }

    public static string CheckName(string? name)
    {
        while (!Regex.IsMatch(name!, "^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect name format, try again please: ");
            name = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Name: {name}");

        return name!;
    }

    public static string CheckPassNum(string? passNum)
    {
        string[] regCodes = {"AB", "BM", "HB", "KH", "MP", "MC", "KB"};
        if (passNum != "PP")
            while (true)
            {
                if (regCodes.Contains(passNum![..2]))
                    if (Regex.IsMatch(passNum[2..], "^\\d+$"))
                        if (passNum.Length == 9)
                            break;

                MenuController.Clear(1);
                Console.Write("✘ Incorrect passport No' format, try again please: ");
                passNum = Console.ReadLine();
            }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Passport No': {passNum}");

        return passNum;
    }

    public static string CheckPassId(string? passId)
    {
        char[] years = {'1', '2', '3', '4', '5', '6'};
        char[] region = {'A', 'B', 'C', 'H', 'K', 'E', 'M'};
        string[] nationality = {"PB", "BA", "BI"};

        while (true)
            try
            {
                if (passId!.Length != 14)
                    throw new Exception();
                if (!years.Contains(passId[0]))
                    throw new Exception();
                if (!region.Contains(passId[7]))
                    throw new Exception();
                if (!nationality.Contains(passId.Substring(11, 2)))
                    throw new Exception();
                if (!DataCheck(passId.Substring(1, 2), 1))
                    throw new Exception();
                if (!DataCheck(passId.Substring(3, 2), 2))
                    throw new Exception();
                if (!DataCheck(passId.Substring(5, 2), 3))
                    throw new Exception();
                if (!(Convert.ToInt32(passId.Substring(8, 3)) > 0))
                    throw new Exception();
                if (Convert.ToInt32(passId[13]) == 0)
                    throw new Exception();
                break;
            }
            catch
            {
                MenuController.Clear(1);
                Console.Write("✘ Incorrect passport ID format, try again please: ");
                passId = Console.ReadLine();
            }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Passport ID: {passId}");

        return passId;
    }

    private static bool DataCheck(string dat, int type)
    {
        switch (type)
        {
            case 1:
                if (Convert.ToInt32(dat) is < 1 or > 31)
                    return false;
                break;

            case 2:
                if (Convert.ToInt32(dat) is < 1 or > 12)
                    return false;
                break;

            case 3:
                if (Convert.ToInt32(dat) < 22)
                    if (Convert.ToInt32(dat) + 2000 > 2004)
                        return false;

                break;
        }

        return true;
    }

    public static string CheckPhone(string? phone)
    {
        while (!Regex.IsMatch(phone!, "^\\s*\\+?375((33\\d{7})|(29\\d{7})|(44\\d{7}|)|(25\\d{7}))\\s*$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect phone format, try again please: ");
            phone = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Phone Number: {phone}");

        return phone!;
    }

    public static string CheckEmail(string? email)
    {
        while (!Regex.IsMatch(email!, "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect e-mail format, try again please: ");
            email = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Email: {email}");

        return email!;
    }

    public static string CheckComName(string? comName)
    {
        while (!Regex.IsMatch(comName!, "^(?!\\s)(?!.*\\s$)(?=.*[a-zA-Z0-9])[a-zA-Z0-9 '-?!]{2,}$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect company name format, try again please: ");
            comName = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Company name: {comName}");

        return comName!;
    }

    public static string CheckComType(string? type)
    {
        while (!Regex.IsMatch(type!, "^\\b[^\\d\\W]+\\b$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect company type format, try again please: ");
            type = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Company type: {type}");

        return type!;
    }

    public static string CheckUnp(string? unp)
    {
        while (!Regex.IsMatch(unp!, "^\\d{9}$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect UNP format, try again please: ");
            unp = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Company UNP: {unp}");

        return unp!;
    }

    public static string CheckBic(string? bic)
    {
        while (!Regex.IsMatch(bic!, "^[A-Z]{6,6}[A-Z2-9][A-NP-Z0-9]([A-Z0-9]{3,3}){0,1}$"))
        {
            MenuController.Clear(1);
            Console.Write("✘ Incorrect BIC format, try again please: ");
            bic = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Company BIC: {bic}");

        return bic!;
    }

    public static string CheckComAddress(string? comAdd)
    {
        while (comAdd == string.Empty)
        {
            MenuController.Clear(1);
            Console.Write("✘ Company address must not be empty, try again please: ");
            comAdd = Console.ReadLine();
        }

        MenuController.Clear(1);
        Console.WriteLine($"✔ Company address: {comAdd}");

        return comAdd!;
    }

    public static string CheckMoney(string? money)
    {
        var tries = 1;
        while (!Regex.IsMatch(money!, "^[.\\d]+$"))
        {
            if (tries == 1)
                Console.WriteLine(" ");

            money = TryAgain("✘ Incorrect money format.");
            tries = 2;
        }

        MenuController.Clear(tries);
        Console.WriteLine($"✔ Money sum: {money}");

        return money!;
    }

    public static string CheckAccId(string? id)
    {
        var tries = 1;
        while (!Regex.IsMatch(id!, "^[\\d]+$"))
        {
            Console.WriteLine(" ");
            id = TryAgain("✘ Incorrect id format.");
            tries = 2;
        }

        MenuController.Clear(tries);
        Console.WriteLine($"✔ Beneficiary ID: {id}");

        return id!;
    }

    public static string CheckWorkerId(string? id)
    {
        var tries = 1;

        while (true)
        {
            if (!Regex.IsMatch(id!, "^[\\d]+$"))
            {
                if (tries == 1) Console.WriteLine(" ");

                id = TryAgain("✘ Incorrect id format.");
                tries = 2;
                continue;
            }

            if (AccountController.CheckAccId(Convert.ToInt32(id)).Result == null)
            {
                if (tries == 1) Console.WriteLine(" ");

                id = TryAgain("✘ This account id does not exist.");
                tries = 2;
                continue;
            }

            break;
        }

        MenuController.Clear(tries);
        Console.WriteLine($"✔ Account's ID: {id}");

        return id!;
    }

    public static async Task<string> CheckUserId(string? id)
    {
        var tries = 1;

        while (true)
        {
            if (!Regex.IsMatch(id!, "^[\\d]+$"))
            {
                if (tries == 1) Console.WriteLine(" ");

                id = TryAgain("✘ Incorrect id format.");
                tries = 2;
                continue;
            }

            var res = await MainController.Db!.CheckClId(Convert.ToInt32(id));
            if (res == false)
            {
                if (tries == 1) Console.WriteLine(" ");

                id = TryAgain("✘ This account id does not exist.");
                tries = 2;
                continue;
            }

            break;
        }

        MenuController.Clear(tries);
        Console.WriteLine($"✔ Account's ID: {id}");

        return id!;
    }

    public static string CheckTime(string? time)
    {
        var tries = 1;

        while (true)
        {
            if (!Regex.IsMatch(time!, "^[\\d]+$"))
            {
                if (tries == 1) Console.WriteLine(" ");

                time = TryAgain("✘ Incorrect loan time format.");

                tries = 2;
                continue;
            }

            if (Convert.ToInt32(time) % 6 != 0)
            {
                if (tries == 1) Console.WriteLine(" ");

                time = TryAgain("✘ Invalid loan time value.");

                tries = 2;
                continue;
            }

            if (Convert.ToInt32(time) <= 24)
            {
                if (tries == 1) Console.WriteLine(" ");

                time = TryAgain("✘ Invalid loan time value.");

                tries = 2;
                continue;
            }

            break;
        }

        MenuController.Clear(tries);
        Console.WriteLine($"✔ Selected loan time: {time}");

        return time!;
    }
}