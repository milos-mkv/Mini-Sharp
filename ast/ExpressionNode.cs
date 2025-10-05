using System.Diagnostics.CodeAnalysis;

namespace MiniSharp.ast;

public class ExpressionNode : TermNode
{
    public ASTNode Left { get; set; }
    public string Op { get; set; } = "";
    public ASTNode Right { get; set; }

    public override void Print(string indent)
    {
        Console.WriteLine(indent + "ExpressionNode:"+Line);
        Left.Print(indent + "  ");
        if (!Op.Equals(""))
        {
            Console.WriteLine(indent + "  Operator: " + Op);
            Right.Print(indent + "  ");
        }
        
    }

    public override object? Execute(Context context)
    {
        Debug(context);
        object? left = Left.Execute(context);
        object? result = null;
        if (!Op.Equals("") && Right != null )
        {
            object? right = Right.Execute(context);
            
            if (Op.Equals("+") && left is double l && right is double r)
            {
                result = l + r;
            }
            else if (Op.Equals("-") && left is double l1 && right is double r1)
            {
                result = l1 - r1;
            }
            else if (Op.Equals("*") && left is double l2 && right is double r2)
            {
                result = l2 * r2;
            }
            else if (Op.Equals("/") && left is double l3 && right is double r3)
            {
                result = l3 / r3;
            }
            else if (Op.Equals("^") && left is double l4 && right is double r4)
            {
                result = Math.Pow(l4, r4);
            }
            else if (Op.Equals("<") && left is double l5 && right is double r5)
            {
                result = l5 < r5;
            }
            else if (Op.Equals(">") && left is double l6 && right is double r6)
            {
                result = l6 > r6; 
            }
            else if (Op.Equals("=") && left is double l7 && right is double r7)
            {
                result = r7;
            }
            else if (Op.Equals("!="))
            {
                result = !Equals(left, right);
            }
            else if (Op.Equals("<=") && left is double l9 && right is double r9)
            {
                result = l9 <= r9;
            }
            else if (Op.Equals(">=") && left is double l10 && right is double r10)
            {
                result = l10 >= r10;
            }
            else if (Op.Equals("=="))
            {
                result = Equals(left, right);
            }
            else if (Op.Equals("||"))
            {
                result = Convert.ToBoolean(left) || Convert.ToBoolean(right);
            }     
            else if (Op.Equals("&&"))
            {
                result = Convert.ToBoolean(left) && Convert.ToBoolean(right);
            }
            
        }
        else
        {
            result = left;
        }
        return result;
    }
}