using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using TerrariaRandomizer.Data;

namespace TerrariaRandomizer.Configuration;

public class Parameters
{
    public int Count { get; private set; } = 1;
    public int MaxAnimationFrames { get; private set; } = 150;
    public int MinAnimationFrames => MaxAnimationFrames - 50;
    public bool UseWorldSize { get; private set; } = false;
    public bool UseDifficulty { get; private set; } = false;
    public bool Fast { get; private set; } = false;
    public bool OnlyVanilla { get; private set; } = false;
    public bool OnlyCalamity { get; private set; } = false;
    public bool UseSeeds { get; private set; } = false;
    public bool UseChallenges { get; private set; } = false;
    public bool NoAscii { get; private set; } = false;
    public List<string> DisabledClasses { get; private set; } = new();
    public string Language { get; private set; } = "en";

    public Parameters(string[] args)
    {
        var parser = new Parser(with =>
        {
            with.HelpWriter = null;
            with.AutoHelp = false;
            with.AutoVersion = false;
        });
        var parserResult = parser.ParseArguments<Options>(args).MapResult(options =>
        {
            options.Language = Localization.GetCode(options.Language);
            if (!Localization.GetLanguages().Contains(options.Language))
            {
                UI.PrintError("Parameters.Language.Error".Localize("en"));
                return 1;
            }
            UI.Language = Language = options.Language;
            if (options.Count > 1) options.Fast = true;
            if (options.Count > 12)
            {
                UI.PrintError("Parameters.Count.Error".Localize(Language));
                return 1;
            }
            Count = options.Count;
            if (options.MaxAnimationFrames < 55)
            {
                UI.PrintError("Parameters.MaxAnimationFrames.Error".Localize(Language));
                return 1;
            }
            MaxAnimationFrames = options.MaxAnimationFrames;
            Fast = options.Fast;
            if (options.OnlyVanilla && options.OnlyCalamity)
            {
                UI.PrintError("Parameters.Only.Error".Localize(Language));
                return 1;
            }
            OnlyVanilla = options.OnlyVanilla;
            OnlyCalamity = options.OnlyCalamity;
            UseSeeds = options.UseSeeds;
            UseChallenges = options.UseChallenges;
            UseWorldSize = options.UseWorldSize;
            UseDifficulty = options.UseDifficulty;
            UI.NoAscii = NoAscii = options.NoAscii;
            if (options.NoColor) UI.NoColor();
            if (options.DisabledClasses?.Any() == true)
            {
                var validClasses = Constants.Classes.Concat(new[] { "Rogue" }).ToList();
                DisabledClasses = options.DisabledClasses.Distinct().ToList();
                var invalidClasses = DisabledClasses.Where(c => !validClasses.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
                if (invalidClasses.Any())
                {
                    UI.PrintError("Parameters.InvalidClasses.Error".Localize(Language) + string.Join(", ", invalidClasses));
                    return 1;
                }
                if (DisabledClasses.Count == validClasses.Count)
                {
                    UI.PrintError("Parameters.AllClassesDisabled.Error".Localize(Language));
                    return 1;
                }
            }
            if (options.Version) UI.PrintVersion();
            if (options.ListClasses) UI.PrintListClasses(Constants.Classes.Concat(new[] { "Rogue (Calamity only)" }).ToList());
            if (options.ListLanguages) UI.PrintListLanguages(Localization.GetLanguagesWithName());
            if (options.Help) UI.PrintHelp();
            return 0;
        },
        errors =>
        {
            UI.PrintError("Parameters.InvalidInput.Error".Localize(Language));
            return -1;
        });
    }
}
