
namespace MiniSharp.ast;

public class AnonymousFunctionNode : TermNode
{
    public List<string> Parameters { get; set; } = [];
    public StatementsNode Body { get; set; } = new();
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "AnonymousFunctionNode:");
        foreach (var parameter in Parameters)
        {
            Console.WriteLine(indent + "Param: " + parameter);
        }
        Body.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        return new UserFunction(Parameters, Body);
    }
}