namespace MiniSharp;

public static class Tokens 
{
    public static readonly HashSet<string> KeywordConstants = 
    [
        "true", "false", "this", "null"
    ];

    public static readonly HashSet<string> Keywords =
    [
        "class", "constructor", "function", "method", "field", "var", "static", "number",
        "string", "boolean", "void", "true", "false", "null", "this", "let", "do",
        "if", "else", "while", "return", "for", "break", "continue"
    ];

    public static readonly HashSet<string> Types = 
    [
        "number", "void", "boolean", "string"
    ];

    public static readonly HashSet<string> Symbols = 
    [
        "{", "}", "(", ")", "[", "]", ".", ",", ";", "~",
        "+", "-", "*", "/", "&", "|", "<", ">", "=", "==", "!=", "<=", ">=", "||", "&&", "^"
    ];

    public static readonly HashSet<char> Skipable = 
    [
        '"', '\'', ' ', '\t', '\n'
    ];

    public static readonly HashSet<string> Operators = 
    [
        "+", "-", "*", "/", "&", "|", "<", ">", "=", "==", "!=", "<=", ">=", "||", "&&", "^", "->"
    ];
    
    public static readonly HashSet<string> UnaryOperators = 
    [
        "-", "~"
    ];
}