namespace MiniSharp;

public class Token(string type, string value, int line)
{
    public string Type { get; set; } = type;
    public string Value { get; set; } = value;
    public int Line { get; set; } = line;

    public string ToXmlStr()
    {
        return "<" + Type + ">" + Value + "</" + Type + ">";
    }
}