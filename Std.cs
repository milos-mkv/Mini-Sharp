namespace MiniSharp;

public class Std
{
    public void Print(params object[] args)
    {
        Console.WriteLine(string.Join(", ", args));
    }

    public string Read()
    {
        return Console.ReadLine();
    }
}