# Brick by Brick – SkillPoints (Mod)

Press **F10** in a world you host: **+10 skill points**. Works in every world and save slot.

> Unofficial fan mod, not made by the game's developers. Once you use it in a world, the game no longer counts
> that world for the Steam leaderboard (the game does this itself for any granted skill points).

## Install

1. Download **[skillpoints-mod.zip](https://github.com/michel86ch-ui/brickbybrick-mod-skillpoints/raw/main/skillpoints-mod.zip)**.
2. Unzip everything into the game folder, so that `winhttp.dll` sits next to `BrickByBrick.exe`
   (Steam: right-click the game > Manage > Browse local files). BepInEx already installed (e.g. with
   [Extra Buildings](https://github.com/michel86ch-ui/brickbybrick-mod))? Just overwrite, the files are the same.

Key and amount: `BepInEx\config\michel.brickbybrick.skillpoints.cfg` (created at the first game start).
Uninstall: delete the folder `BepInEx\plugins\SkillPoints`.

The ZIP contains BepInEx 5.4.23.5 (unmodified, MIT license) and the mod.

## Building

```
cd src
dotnet build -c Release -p:DeployToGame=true     (copies the DLL into the game)
```

The mod calls the game's own `ProgressionService.GrantCurrencyForDevelopment("skill-point", n)`; only the host can
change progression.
