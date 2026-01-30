using TerrariaRandomizer.Configuration;
using TerrariaRandomizer.Data;

namespace TerrariaRandomizer;

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
        string gameType = parameters.OnlyVanilla ? "Vanilla" : parameters.OnlyCalamity ? "Calamity" : Random(rand, Constants.GameTypes);
        string name = Random(rand, Constants.Names);
        var availableClasses = Constants.Classes.ToList();
        if (gameType == "Calamity") availableClasses.Add("Rogue");
        var validClasses = availableClasses.Where(c => !parameters.DisabledClasses.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
        string characterClass = null;
        if (!validClasses.Any()) UI.PrintError("GenerateCharacter.ValidClasses.Error".Localize(parameters.Language));
        else characterClass = validClasses[rand.Next(validClasses.Count)];
        string evil = Random(rand, Constants.Contagions);
        string challenge = Random(rand, Constants.Challenges);
        string seed = Random(rand, Constants.SpecialSeeds);
        string mapSize = parameters.UseWorldSize ? Random(rand, Constants.MapSizes) : Constants.MapSizeDefault;
        string difficulty = parameters.UseDifficulty ? Random(rand, Constants.Difficultys) : Constants.DifficultyLevelDefault;
        string calamityDifficulty = parameters.UseDifficulty ? Random(rand, Constants.CalamityDifficultys) : Constants.CalamityDifficultyDefault;
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
