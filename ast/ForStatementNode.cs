namespace MiniSharp.ast;

public class ForStatementNode : ASTDebug
{
    public ForConditionNode ForCondition { get; set; }
    public StatementsNode Statements { get; set; }
    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ForStatementNode:"+Line);
        ForCondition.Print(indent + "  ");
        Statements.Print(indent + "  ");
    }

    public override List<object> Execute(Context context)
    {
        Debug(context);
        var varName = ForCondition.VarName;
        object? start = ForCondition.Start.Execute(context);
        object? end = ForCondition.End.Execute(context);
        object? increment = ForCondition.Increment.Execute(context);
        
        double i1 = Convert.ToDouble(start);
        double i2 = Convert.ToDouble(end);
        double i3 = Convert.ToDouble(increment);
        context.Define(varName, i1);
        
        while (Convert.ToBoolean( i1 < i2))
        {
            try
            {
                Statements.Execute(context);

            }
            catch (BreakException e)
            {
                break;
            }
            catch (ContinueException e)
            {
                
            }
            i1 += i3;
            context.Set(varName, i1);
        }
        return null;
    }
}