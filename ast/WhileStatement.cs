namespace MiniSharp.ast;

public class WhileStatement : ASTDebug
{
    public ExpressionNode Condition { get; set; }
    public StatementsNode? Statements { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "WhileStatement:" + Line);
        Condition.Print(indent + "  ");
        Statements?.Print(indent + "  ");
        
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        while(Convert.ToBoolean(Condition.Execute(context)))
        {
            try
            {
                var currentContext = new Context(context);
                Statements?.Execute(currentContext);
                Debug(context);
            }
            catch (BreakException e)
            {
                break;
            }
            catch (ContinueException e)
            {
                continue;
            }
        }
        return null;
    }
}