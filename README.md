# ULTRALOADER
A mod loader for ULTRAKILL

## TweakIt
My own mod is also included in the same solution, TweakIt, which adds
miscellaneous tweaks.

## Installation
Download the latest release ZIP file, and extract all of it's contents into the
root game directory, where `ULTRAKILL.exe` is. `dll`s inside of the `Mods`
directory will automatically be loaded -- this is where you should put
`TweakIt.dll` and others!

## Build
Both projects expect the game directory to be in the `Game` directory. (as in,
`Game/ULTRAKILL`). You can (and obviously should) symlink it.

After this, all you need is .NET Framework 4.0, and build.

Put `winhttp.dll` (bring your own), `doorstop_config.ini`, `ULTRALOADER.dll`,
and `0Harmony.dll` next to the game executable. Start the game normally.
