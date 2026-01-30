using System;
using System.Collections.Generic;
using BeautifulConsole.GUI;
using Spectre.Console;

namespace TerrariaRandomizer.Configuration.Settings;

public static class SettingsUI
{
    private static int SelectedIndex = 0;

    public static Options Show(Options options)
    {
        UI.PrintTitleNoAscii();
        List<Slider> sliders = CreateSliders(options);
        var rule = new Rule("[silver]  ↑↓ W/S  Select    ←→ A/D  Change    ↵ Enter/Esc Exit[/]");
        rule.Style = Style.Parse("grey");
        UpdateSliders(sliders, 0);
        while (true)
        {
            Console.Write($"\x1b[5;0H");
            for (int i = 0; i < sliders.Count; i++) sliders[i].Draw(i == SelectedIndex, 64);
            Console.Write($"\x1b[18;0H");
            AnsiConsole.Write(rule);
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow or ConsoleKey.W: MoveSelection(sliders, -1); break;
                case ConsoleKey.DownArrow or ConsoleKey.S: MoveSelection(sliders, 1); break;
                case ConsoleKey.LeftArrow or ConsoleKey.A: UpdateSliders(sliders, -1); break;
                case ConsoleKey.RightArrow or ConsoleKey.D: UpdateSliders(sliders, 1); break;
                case ConsoleKey.Enter:
                    UpdateOptionsFromSliders(options, sliders);
                    return options;
                case ConsoleKey.Escape: return options;
            }
        }
    }

    private static void MoveSelection(List<Slider> sliders, int direction)
    {
        int newIndex = SelectedIndex;
        int attempts = 0;
        do
        {
            newIndex = (newIndex + direction + sliders.Count) % sliders.Count;
            attempts++;
            if (attempts > sliders.Count) return;
        }
        while (!sliders[newIndex].Enabled);
        SelectedIndex = newIndex;
    }

    private static void UpdateSliders(List<Slider> sliders, int direction)
    {
        if (direction == -1) sliders[SelectedIndex].Decrease();
        else if (direction == 1) sliders[SelectedIndex].Increase();
        if ((int)sliders[0].Value > 1)
        {
            sliders[2].Disable();
            sliders[2].Value = true;
        }
        else sliders[2].Enable();
        sliders[1].Enabled = !sliders[2].Value;
        if ((bool)sliders[3].Value) sliders[4].Disable();
        else sliders[4].Enable();
        if ((bool)sliders[4].Value) sliders[3].Disable();
        else sliders[3].Enable();
    }

    private static List<Slider> CreateSliders(Options options) =>
        new List<Slider>
        {
            new Slider("Count", SliderValueTypes.Integer, options.Count, 1, 12),
            new Slider("MaxAnimationFrames", SliderValueTypes.Integer, options.MaxAnimationFrames, 55, 256),
            new Slider("Fast", SliderValueTypes.Boolean, options.Fast) {Looped = true},
            new Slider("OnlyVanilla", SliderValueTypes.Boolean, options.OnlyVanilla) {Looped = true},
            new Slider("OnlyCalamity", SliderValueTypes.Boolean, options.OnlyCalamity) {Looped = true},
            new Slider("UseSeeds", SliderValueTypes.Boolean, options.UseSeeds) {Looped = true},
            new Slider("UseChallenges", SliderValueTypes.Boolean, options.UseChallenges) {Looped = true},
            new Slider("UseWorldSize", SliderValueTypes.Boolean, options.UseWorldSize) {Looped = true},
            new Slider("UseDifficulty", SliderValueTypes.Boolean, options.UseDifficulty) {Looped = true},
            new Slider("NoAscii", SliderValueTypes.Boolean, options.NoAscii) {Looped = true},
            new Slider("NoColor", SliderValueTypes.Boolean, options.NoColor) {Looped = true}
        };

    private static void UpdateOptionsFromSliders(Options options, List<Slider> sliders)
    {
        options.Count = (int)sliders[0].Value;
        options.MaxAnimationFrames = (int)sliders[1].Value;
        options.Fast = (bool)sliders[2].Value;
        options.OnlyVanilla = (bool)sliders[3].Value;
        options.OnlyCalamity = (bool)sliders[4].Value;
        options.UseSeeds = (bool)sliders[5].Value;
        options.UseChallenges = (bool)sliders[6].Value;
        options.UseWorldSize = (bool)sliders[7].Value;
        options.UseDifficulty = (bool)sliders[8].Value;
        options.NoAscii = (bool)sliders[9].Value;
        options.NoColor = (bool)sliders[10].Value;
    }
}
