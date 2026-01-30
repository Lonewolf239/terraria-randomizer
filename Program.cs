using TerrariaRandomizer.Configuration;

namespace TerrariaRandomizer;

public class Program
{
    public static bool IsBeyondAsciiRange { get; private set; } = false;

    private static async Task Main(string[] args)
    {
        UI.Clear();
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        if (Console.WindowWidth < 85) IsBeyondAsciiRange = true;
        var parameters = new Parameters(args);
        var rand = new Random(Guid.NewGuid().GetHashCode());
        UI.PrintTitle();
        do
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            if (!parameters.Fast)
            {
                int frameCount = rand.Next(parameters.MinAnimationFrames, parameters.MaxAnimationFrames);
                for (int frame = 0; frame < frameCount; frame++)
                {
                    await Generator.Generate(parameters, rand);
                    await Task.Delay(5);
                }
            }
            await Generator.Generate(parameters, rand);
            UI.PrintRule();
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar) break;
                if (key == ConsoleKey.Escape)
                {
                    UI.Clear();
                    Console.CursorVisible = true;
                    return;
                }
            }
            UI.Clear();
            UI.PrintTitle();
        } while (true);
    }
}
