using System;
using System.Linq;
using System.Threading.Tasks;

namespace TerrariaRandomizer;

public class GenerationResult
{
    public string GameType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CharacterClass { get; set; } = string.Empty;
    public string Evil { get; set; } = string.Empty;
    public string Challenge { get; set; } = string.Empty;
    public string Seed { get; set; } = string.Empty;
    public string MapSize { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string CalamityDifficulty { get; set; } = string.Empty;
}

public static class Generator
{
    public static async Task Generate(Parameters parameters, Random rand)
    {
        int y = parameters.NoAscii ? 5 : 18;
        Console.Write($"\x1b[{y};0H");
        for (int i = 0; i < parameters.Count; i++)
        {
            UI.PrintResults(parameters, await GenerateCharacter(parameters, rand));
            Console.WriteLine();
        }
    }

    public static async Task<GenerationResult> GenerateCharacter(Parameters parameters, Random rand)
    {
        string gameType = parameters.OnlyVanilla ? "Vanilla" : parameters.OnlyCalamity ? "Calamity" : Random(rand, parameters.GameTypes);
        string name = Random(rand, parameters.Names);
        var availableClasses = parameters.Classes.ToList();
        if (gameType == "Calamity") availableClasses.Add("Rogue");
        var validClasses = availableClasses.Where(c => !parameters.DisabledClasses.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
        string characterClass = null;
        if (!validClasses.Any())
        {
            if (parameters.Russian) UI.PrintError("Нет доступных классов персонажей!");
            else UI.PrintError("No character classes available!");
        }
        else characterClass = validClasses[rand.Next(validClasses.Count)];
        string evil = Random(rand, parameters.Contagions);
        string challenge = Random(rand, parameters.Challenges);
        string seed = Random(rand, parameters.SpecialSeeds);
        string mapSize = parameters.UseWorldSize ? Random(rand, parameters.MapSizes) : parameters.MapSizeDefault;
        string difficulty = parameters.UseDifficulty ? Random(rand, parameters.Difficultys) : parameters.DifficultyLevelDefault;
        string calamityDifficulty = parameters.UseDifficulty ? Random(rand, parameters.CalamityDifficultys) : parameters.CalamityDifficultyDefault;
        return new GenerationResult
        {
            GameType = gameType,
            Name = name,
            CharacterClass = characterClass,
            Evil = evil,
            Challenge = challenge,
            Seed = seed,
            MapSize = mapSize,
            Difficulty = difficulty,
            CalamityDifficulty = calamityDifficulty
        };
    }

    private static string Random(Random rand, string[] array) => array[rand.Next(array.Length)];
}
