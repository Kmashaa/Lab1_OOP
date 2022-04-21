namespace Lab1.Models;

public abstract class Menu
{
    protected readonly Dictionary<string, Choice> Action = new();
    protected bool Ext = false;

    protected Menu(List<string> controls, string info)
    {
        Controls = controls;
        Info = info;
    }

    public string Info { get; }

    public List<string> Controls { get; protected init; }
    public User? Session { get; protected set; }
    public int ChosenBank { get; protected set; }

    protected delegate void Choice();
}