namespace MiniSharp.ast;

public abstract class ASTDebug : ASTNode
{
    public void Debug(Context context)
    {
        // Debugging hook
        if (context.GetDebugger() != null && context.GetBreakpoints().Contains(Line))
        {
            if (context.LastBreakLine != Line) // prevent duplicate breaks
            {
                context.LastBreakLine = Line;
                context.Debugger.OnBreak(Line, this, context);
            }
     
        }
    }
}