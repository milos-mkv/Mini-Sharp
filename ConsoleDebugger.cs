using MiniSharp.ast;

namespace MiniSharp;

public class ConsoleDebugger : IDebugger
{
    public bool ShouldBreak(int line, ASTNode node, Context context)
    {
        return true; // always break on lines in Breakpoints
    }

    public void OnBreak(int line, ASTNode node, Context context)
    {
        Console.WriteLine($"[BREAK] Line {line}: {node.GetType().Name}");
        Console.WriteLine("Variables:");
        context.PrintVars();

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}