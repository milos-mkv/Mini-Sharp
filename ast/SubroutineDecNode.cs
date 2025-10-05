namespace MiniSharp.ast;

public class SubroutineDecNode : ASTDebug
{
    public string Name {  get; set; }
    public List<string> Params { get; set; } = new();
    
    public StatementsNode Statements { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "SubroutineDecNode: " + Name + " LINE " + Line);
        foreach (var param in Params)
        {
            Console.WriteLine(indent + "  Param: " + param);
        }
        Statements?.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        var func = new UserFunction(Params, Statements);
        context.Define(Name, func);
        return null;
    }
}