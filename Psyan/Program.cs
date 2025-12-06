using Spectre.Console.Cli;


namespace Psyan;




class Program
{
    public static void Main(string[] args)
    {
        args = "-i".Split(' ');

        var root = new CommandApp<PisanCommand>();
        root.Run(args);
    }
}
