namespace MiniSharp;

public class MiniSharpInterpreter
{
    public MiniSharpInterpreter()
    {
        Console.WriteLine("SmallLanguageInterpreter");
    }

    public void executeFile(string filename)
    {
        try
        {
            // read all text from the file
            var code = File.ReadAllText(filename);

            // create tokenizer with filename + code
            var tokenizer = new Tokenizer(filename, code);
                
            var parser = new Parser(tokenizer);
            
            var context = new Context();
            context.Debugger = new ConsoleDebugger();
            context.Define("Std", new Std());
            context.Define("print", new NativeFunction(args =>
            {
                Console.WriteLine(string.Join(" ", args));
                return null;
            }));
            var value = parser.ProgramNode.Execute(context);
            Console.WriteLine(value);
          //  var i = 10;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error reading file {filename}: {e.Message}");
        }
    }
}