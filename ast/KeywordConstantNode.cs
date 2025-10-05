namespace MiniSharp.ast;

public class KeywordConstantNode(string value) : TermNode
{
    public string Value { get; set; } = value;

    public override void Print(string indent)
    {
        Console.WriteLine(indent + "KeywordConstantNode: " +  Value + " LINE " + Line);
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        var result = Value switch
        {
            "null" => null,
            "true" => true,
            "false" => false,
            "this" => context.Get("this"),
            _ => null
        };
        return result;
    }
}