using MiniSharp.ast;

namespace MiniSharp;

public class Parser
{
    private readonly Tokenizer _tokenizer;
    
    public ProgramNode ProgramNode { get; private set; } = new ProgramNode();

    public Parser(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
        var node = CompileStatements();
        ProgramNode.Statements = node;
        ProgramNode.Print("");
    }

    private StatementsNode CompileStatements(string end = "EOF")
    {
        StatementsNode node = new StatementsNode();
        while (_tokenizer.Next().Value != end)
        {
            var statementType = _tokenizer.Next().Value;

            if (statementType == "let")
            {
                var statement = CompileLetStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "if")
            {
                var statement = CompileIfStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "while")
            {
                var statement = CompileWhileStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "for")
            {
                var statement = CompileForStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "do")
            {
                var statement = CompileDoStatement();
                node.Statements.Add(statement);    
            }
            else if (statementType == "function")
            {
                var statement = CompileSubroutineDec();
                node.Statements.Add(statement);
            }
            else if (statementType == "return")
            {
                var statement = CompileReturnStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "break")
            {
                var statement = CompileBreakStatement();
                node.Statements.Add(statement);
            }
            else if (statementType == "continue")
            {
                var statement = CompileContinueStatement();
                node.Statements.Add(statement);
            }
            else
            {
                var statement = CompileAssignment();
                node.Statements.Add(statement);
            }
        }

        return node;
    }

    BreakStatementNode CompileBreakStatement()
    {
        var node = new BreakStatementNode();
        CheckForValue("break");
        CheckForValue(";");
        return node;
    }

    ContinueStatementNode CompileContinueStatement()
    {
        var node = new ContinueStatementNode();
        CheckForValue("continue");
        CheckForValue(";");
        return node;
    }

    ReturnStatementNode CompileReturnStatement()
    {
        var node = new ReturnStatementNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("return");
        node.Expression = CompileExpression();
        CheckForValue(";");
        return node;
    }
    
    SubroutineDecNode CompileSubroutineDec()
    {
        var node = new SubroutineDecNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("function");
        node.Name = CheckForIdentifier();
        CheckForValue("(");
        while (_tokenizer.Next().Value != ")")
        {
            node.Params.Add(CheckForIdentifier());
            if (_tokenizer.Next().Value == ")")
            {
                break;
            }
            CheckForValue(",");
        }
        CheckForValue(")");
        if (_tokenizer.Next().Value == "->")
        {
            CheckForValue("->");
            node.Statements = new StatementsNode();
            ReturnStatementNode returnNode = new ReturnStatementNode();
            returnNode.Expression = CompileExpression();
            node.Statements.Statements.Add(returnNode);
            CheckForValue(";");
        }
        else
        {
            CheckForValue("{");
            node.Statements = CompileStatements("}");
            CheckForValue("}");
        }
        
        return node;
    }
    
    DoStatementNode CompileDoStatement()
    {
        DoStatementNode node = new DoStatementNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("do");
        node.Call = CompileSubroutineCall();
        CheckForValue(";");
        return node;
    }

   SubroutineCallNode CompileSubroutineCall()
    {
        int line = _tokenizer.Next().Line;

        // Start with a general term — not just an identifier
        TermNode term = CompileTerm();

        // Now we might have more chain parts: ".", "[", or "()" directly
        while (true)
        {
            var next = _tokenizer.Next()?.Value;

            // member access (obj.field or obj.method())
            if (next == ".")
            {
                CheckForValue(".");
                string member = CheckForIdentifier();

                if (_tokenizer.Next()?.Value == "(")
                {
                    // method call on member
                    var callNode = new SubroutineCallNode
                    {
                        Line = _tokenizer.Next().Line,
                        FunctionName = member,
                        Target = term
                    };
                    CheckForValue("(");
                    callNode.ExpressionList = CompileExpressionList();
                    CheckForValue(")");
                    term = callNode;
                }
                else
                {
                    // just property access
                    term = new MemberAccessNode
                    {
                        Line = _tokenizer.Next().Line,
                        Left = term,
                        Member = member
                    };
                }
            }
            // array access (arr[expr])
            else if (next == "[")
            {
                CheckForValue("[");
                var indexExpr = CompileExpressionList();
                CheckForValue("]");

                term = new ArrayAccessNode
                {
                    Line = _tokenizer.Next().Line,
                    Target = term,
                    Index = indexExpr
                };
            }
            // direct call (arr[0]() or (() -> {...})())
            else if (next == "(")
            {
                var callNode = new SubroutineCallNode
                {
                    Line = _tokenizer.Next().Line,
                    Target = term,
                    FunctionName = null
                };
                CheckForValue("(");
                callNode.ExpressionList = CompileExpressionList();
                CheckForValue(")");
                term = callNode;
            }
            else
            {
                break;
            }
        }

        // If the final result isn’t a SubroutineCallNode yet, wrap it
        if (term is not SubroutineCallNode call)
        {
            throw new Exception("Invalid subroutine call syntax after 'do'");
        }

        return call;
    }



    ExpressionListNode CompileExpressionList()
    {
        var node = new ExpressionListNode();
        node.Line = _tokenizer.Next().Line;
        if (_tokenizer.Next().Value != ")")
        {
            node.Expressions.Add(CompileExpression());
            while (_tokenizer.Next().Value == ",")
            {
                CheckForValue(",");
                node.Expressions.Add(CompileExpression());
            }
        }
        return node;
    }
    
    ForStatementNode CompileForStatement()
    {
        ForStatementNode node = new ForStatementNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("for");
        CheckForValue("(");
        var condition = new ForConditionNode();
        condition.Line = _tokenizer.Next().Line;
        CheckForValue("let");
        condition.VarName = CheckForIdentifier();
        CheckForValue("=");
        condition.Start = CompileTerm();
        CheckForValue(",");
        condition.End = CompileTerm();
        CheckForValue(",");
        condition.Increment = CompileTerm();
        CheckForValue(")");
        CheckForValue("{");
        node.Statements = CompileStatements("}");
        CheckForValue("}");
        node.ForCondition = condition;
        return node;
    }
    
    WhileStatement CompileWhileStatement()
    {
        var node = new WhileStatement();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("while");
        CheckForValue("(");
        node.Condition = CompileExpression();
        CheckForValue(")");
        CheckForValue("{");
        node.Statements = CompileStatements("}");
        CheckForValue("}");
        return node;
    }
    
    IfStatementNode CompileIfStatement()
    {
        var node = new IfStatementNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("if");
        CheckForValue("(");
        node.Condition = CompileExpression();
        CheckForValue(")");
        CheckForValue("{");
        node.Statements = CompileStatements("}");
        CheckForValue("}");
        if (_tokenizer.Next().Value == "else")
        {
            CheckForValue("else");
            if (_tokenizer.Next().Value == "if")
            {
                node.Else = CompileIfStatement();
            }
            else
            {
                CheckForValue("{");
                node.Else = CompileStatements("}");
                CheckForValue("}");
            }
        }
        return node;
    }
    
    LetStatementNode CompileLetStatement()
    {
        var node = new LetStatementNode();
        node.Line = _tokenizer.Next().Line;
        CheckForValue("let");
        int line = _tokenizer.Next().Line;
        node.VarName = new VarNameNode(CheckForIdentifier());
        node.VarName.Line = line;
        if (_tokenizer.Next().Value == ";")
        {
            CheckForValue(";");
        }
        else if (_tokenizer.Next().Value == "=")
        {
            CheckForValue("=");
            node.Expression = CompileExpression();
            CheckForValue(";");
        }
        else if (_tokenizer.Next().Value == ",")
        {
            
        }
        
        return node;
    }

    AssignmentNode CompileAssignment()
    {
        var node = new AssignmentNode();
        node.Line = _tokenizer.Next().Line;

        // Instead of forcing identifier, parse a full left-hand expression
        var left = CompileTerm();

        // Ensure the next token is "="
        CheckForValue("=");

        node.Target = left;  // store full left-hand AST node
        node.Expression = CompileExpression();
        CheckForValue(";");

        return node;
    }


    ExpressionNode CompileExpression()
    {
        var node = new ExpressionNode();
        node.Line = _tokenizer.Next().Line;
        node.Left = CompileTerm();
        while (Tokens.Operators.Contains(_tokenizer.Next().Value))
        {
            string op = CheckForOperator();
            int line = _tokenizer.Next().Line;
            var right = CompileTerm();
            node = new ExpressionNode {
                Left = node,
                Op = op,
                Right = right
            };
            node.Line = line;
        }
        return node;
    }

    TermNode CompileTerm()
    {
        TermNode node;
        if (_tokenizer.Next()?.Type == "numberConstant")
        {
            var token = _tokenizer.Advance();
            node = new NumberConstantNode(token?.Value);
            node.Line = token.Line;
        }
        else if (_tokenizer.Next().Type == "stringConstant")
        {
            var token = _tokenizer.Advance();
            node = new StringConstantNode(token?.Value);
            node.Line = token.Line;
        }
        else if (Tokens.KeywordConstants.Contains(_tokenizer.Next().Value))
        {
            var token = _tokenizer.Advance();
            node = new KeywordConstantNode(token.Value);
            node.Line = token.Line;
        }
        else if (Tokens.UnaryOperators.Contains(_tokenizer.Next().Value))
        {
            var token = _tokenizer.Advance();
            node = new UnaryOpNode(token.Value, CompileTerm());
            node.Line =  token.Line;
        }
        else if (_tokenizer.Next().Value == "(")
        {
            int line = _tokenizer.Next().Line;
            CheckForValue("(");

            // Peek ahead to see if this is a lambda parameter list
            var paramsList = new List<string>();
            bool isLambda = false;

            // If we have identifiers separated by commas and then a ')', followed by '->'
            if (_tokenizer.Next()?.Type == "identifier" || _tokenizer.Next()?.Value == ")")
            {
                // Parse parameter list
                while (_tokenizer.Next()?.Value != ")")
                {
                    var param = CheckForIdentifier();
                    paramsList.Add(param);
                    if (_tokenizer.Next()?.Value == ",")
                        CheckForValue(",");
                    else
                        break;
                }
                CheckForValue(")");

                // Check if followed by ->
                if (_tokenizer.Next()?.Value == "->")
                {
                    isLambda = true;
                    CheckForValue("->");
                }
            }

            if (isLambda)
            {
                // Anonymous function detected
                var lambda = new AnonymousFunctionNode { Line = line, Parameters = paramsList };

                CheckForValue("{");
                lambda.Body = CompileStatements("}"); // existing function that parses block statements
                CheckForValue("}");

                node = lambda;
            }
            else
            {
                // Fallback: regular parenthesized expression
                node = CompileExpression();
                CheckForValue(")");
            }
        }
        else if (_tokenizer.Next().Value == "[")
        {
            int line = _tokenizer.Next().Line;
            CheckForValue("[");
            // parse elements
            var elements = new List<ExpressionNode>();

            // Empty array literal
            if (_tokenizer.Next()?.Value != "]")
            {
                while (true)
                {
                    var expr = CompileExpression();
                    elements.Add(expr);

                    if (_tokenizer.Next()?.Value == ",")
                    {
                        CheckForValue(",");
                        continue;
                    }
                    break;
                }
            }

            CheckForValue("]");

            node = new ArrayLiteralNode { Elements = elements, Line = line };
        }
        else
        {
            int line = _tokenizer.Next().Line;
            var identifier = CheckForIdentifier();
            node = new VarNameNode(identifier);
            node.Line = line;

            // handle potential direct call (global function)
            if (_tokenizer.Next()?.Value == "(")
            {
                var subCall = new SubroutineCallNode
                {
                    FunctionName = identifier,
                    ExpressionList = null
                };
                CheckForValue("(");
                subCall.ExpressionList = CompileExpressionList();
                CheckForValue(")");
                node = subCall;
            }

            // unified chain: handles ".", "[", repeatedly
            while (true)
            {
                var next = _tokenizer.Next()?.Value;
                if (next == ".")
                {
                    CheckForValue(".");
                    var memberName = CheckForIdentifier();

                    if (_tokenizer.Next()?.Value == "(")
                    {
                        var callNode = new SubroutineCallNode
                        {
                            FunctionName = memberName,
                            Target = node
                        };
                        CheckForValue("(");
                        callNode.ExpressionList = CompileExpressionList();
                        CheckForValue(")");
                        node = callNode;
                    }
                    else
                    {
                        var memberNode = new MemberAccessNode
                        {
                            Left = node,
                            Member = memberName
                        };
                        node = memberNode;
                    }
                }
                else if (next == "[")
                {
                    CheckForValue("[");
                    var indexExpr = CompileExpressionList();
                    CheckForValue("]");

                    var arrayAccess = new ArrayAccessNode
                    {
                        Target = node,
                        Index = indexExpr
                    };
                    node = arrayAccess;
                }
                else if (next == "(")
                {
                    CheckForValue("(");
                    var callNode = new SubroutineCallNode
                    {
                        Target = node,              // anything callable, not just identifiers
                        FunctionName = null,
                        ExpressionList = CompileExpressionList()
                    };
                    CheckForValue(")");
                    node = callNode;
                }
                else
                {
                    break; // no more chain parts
                }
            }
        }
        return node;
    }

    private void CheckForValue(string value)
    {
        var token = _tokenizer.Advance();
        if (token?.Value != value)
        {
            throw new Exception("Expected " + value + " but got " + token?.Value);
        }
    }

    private string? CheckForIdentifier()
    {
        var token = _tokenizer.Advance();
        return token?.Type != "identifier" ? throw new Exception("Expected type identifier but got " + token?.Type) : token?.Value;
    }

    string CheckForOperator()
    {
        var token = _tokenizer.Advance();
        if (!Tokens.Operators.Contains(token.Value))
        {
            throw new Exception("Expected " + token.Value + " but be operator!");
        }
        return token.Value;
    }
}