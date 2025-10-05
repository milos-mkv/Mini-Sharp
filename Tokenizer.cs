using System.Text;

namespace MiniSharp;

public class Tokenizer 
{
    private readonly string _code;
    private readonly string _file;
    private int _index;
    private readonly List<Token> _tokens;

    public Tokenizer(string file, string code) 
    {
        _file = file;
        _code = code;
        _index = 0;
        _tokens = [];
        FindAllTokens();
    }

    public Token? Advance()
    {
        return _index == _tokens.Count ? null : _tokens[_index++];
    }

    public Token? Next()
    {
        return _index + 1 > _tokens.Count ? null : _tokens[_index];
    }

    public bool HasMoreTokens() => _index < _tokens.Count;

    private static bool IsValidIdentifier(string word) 
    {
        if (string.IsNullOrEmpty(word)) return false;
        return !char.IsDigit(word[0]) && word.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    private static bool IsNumber(string word)
    {
        return !string.IsNullOrEmpty(word) && word.All(char.IsDigit);
    }

    private void ExportXml() 
    {
        var sb = new StringBuilder();
        sb.AppendLine("<tokens>");
        foreach (var t in _tokens) {
            sb.AppendLine("  " + t.ToXmlStr());
        }
        sb.AppendLine("</tokens>");

        try {
            File.WriteAllText(_file + ".tokens.xml", sb.ToString());
        } catch (Exception e) {
            Console.Error.WriteLine($"Error writing XML: {e.Message}");
        }
    }

    private void FindAllTokens() 
    {
        var word = new StringBuilder();
        var inString = false;
        var line = 1;

        for (var i = 0; i < _code.Length; i++) {
            var ch = _code[i];

            if (ch == '\n') line++;

            // 1. Whitespace outside strings
            if (!inString && ch is ' ' or '\t' or '\n') {
                word.Clear();
                continue;
            }

            // 2. String literal
            if (ch == '"' || inString) {
                word.Append(ch);
                if (ch == '"') inString = !inString;
                if (!inString) {
                    var val = word.Length >= 2 ? word.ToString(1, word.Length - 2) : "";
                    _tokens.Add(new Token("stringConstant", val, line));
                    word.Clear();
                }
                continue;
            }

            // 3. Symbols and operators
            if (Tokens.Symbols.Contains(ch.ToString()) || ch == '!' || ch == '=') {
                if (word.Length > 0) {
                    string w = word.ToString();
                    if (Tokens.Keywords.Contains(w))
                        _tokens.Add(new Token("keyword", w, line));
                    else if (IsNumber(w))
                        _tokens.Add(new Token("numberConstant", w, line));
                    else if (IsValidIdentifier(w))
                        _tokens.Add(new Token("identifier", w, line));
                    word.Clear();
                }

                // check if this + next char form a multi-char operator
                char ch2 = (i + 1 < _code.Length) ? _code[i + 1] : '\0';
                string op = ch.ToString();
                if (ch2 != '\0') {
                    string twoCharOp = op + ch2;
                    if (Tokens.Operators.Contains(twoCharOp)) {
                        _tokens.Add(new Token("operator", twoCharOp, line));
                        i++; // skip next char
                        continue;
                    }
                }

                // fallback: single char operator/symbol
                if (Tokens.Operators.Contains(op))
                    _tokens.Add(new Token("operator", op, line));
                else
                    _tokens.Add(new Token("symbol", op, line));

                continue;
            }

            // 4. Build words
            word.Append(ch);
            var nextCh = (i + 1 < _code.Length) ? _code[i + 1] : ' ';
            if (!Tokens.Symbols.Contains(nextCh.ToString()) && !Tokens.Skipable.Contains(nextCh)) continue;
            {
                if (word.Length <= 0) continue;
                var w = word.ToString();
                if (Tokens.Keywords.Contains(w))
                    _tokens.Add(new Token("keyword", w, line));
                else if (IsNumber(w))
                    _tokens.Add(new Token("numberConstant", w, line));
                else if (IsValidIdentifier(w))
                    _tokens.Add(new Token("identifier", w, line));
                word.Clear();
            }
        }
        
        _tokens.Add(new Token("EOF", "EOF", 0));
        ExportXml();
    }
}
