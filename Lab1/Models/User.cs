namespace Lab1.Models;

public abstract class User
{
    protected int Id { get; init; }
    public string? Name { get; protected init; }

    public virtual List<string>? GetInfo()
    {
        return null;
    }
}