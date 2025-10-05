using MiniSharp.ast;

namespace MiniSharp;

public interface IDebugger
{
    bool ShouldBreak(int line, ASTNode node, Context context);
    void OnBreak(int line, ASTNode node, Context context);
}