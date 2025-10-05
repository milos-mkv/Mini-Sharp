namespace MiniSharp.ast;

public class LetStatementNode : ASTDebug
{
    public VarNameNode VarName { get; set; }
    
    public ExpressionNode? Expression { get; set; }
    
    public override void Print(string indent)
    {
        Console.WriteLine(indent +  "LetStatementNode:" + Line);
        VarName.Print(indent + "  ");
        Expression?.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        var name = VarName.Value.ToString();
        object? value = null;


        value = Expression.Execute(context);

        context.Define(name, value);
        return null;
    }


}