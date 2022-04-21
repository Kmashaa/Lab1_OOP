using Lab1.Models;

namespace Lab1.Controllers;

public static class MenuController
{
    public static void Print(Menu? menu)
    {
        Console.Clear();
        Console.WriteLine(menu!.Info);

        int i;

        for (i = 0; i < menu?.Controls.Count - 1; i++)
            Console.WriteLine($"{1 + i}. ⁍ {menu?.Controls[i]}");
        Console.WriteLine($"{1 + i}. ⦻ {menu?.Controls[i]}");
    }

    public static int MenuChoice(int range)
    {
        var choice = Convert.ToInt32(Console.ReadKey(true).KeyChar);

        var cond1 = choice < 48;
        var cond2 = choice > range + 48;

        while (cond1 || cond2)
        {
            choice = Convert.ToInt32(Console.ReadKey(true).KeyChar);
            cond1 = choice < 48;
            cond2 = choice > range + 48;
        }

        return choice - 48;
    }

    public static void Clear(int n)
    {
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - n);
        var blankLine = new string(' ', 80);
        for (var i = 0; i <= n; i++) Console.WriteLine(blankLine);
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - n - 1);
    }
}