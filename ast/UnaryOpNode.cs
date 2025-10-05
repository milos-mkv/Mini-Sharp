namespace MiniSharp.ast;

public class UnaryOpNode(string op, TermNode term) : TermNode
{
    public string Op { get; set; } = op;
    public TermNode Term { get; set; } = term;
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "UnaryOpNode: " + Op + " LINE " + Line);
        term.Print(indent + "  ");
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        object? value = Term.Execute(context);
        
        if (Op == "-")
        {
            if (value is double d)
            {
                return -d; 
            }
            throw new Exception("Unary - applied to non-numeric type");
        }
        else if (Op == "~")
        {
            if (value is int i)
            {
                return ~i; // bitwise NOT (only makes sense on int types)
            }
            throw new Exception("Unary ~ applied to non-integer type");
        }
        return null;
    }
}