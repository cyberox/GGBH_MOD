# GGBH_MOD

A MelonLoader (Il2Cpp) mod for **Tale of Immortal / 鬼谷八荒** (`guigugame` / `guigubahuang`). It acts as a loader bridge: the game's mod manager sends commands through the game event system, and this mod loads the requested mod DLLs and calls their `ModMain.Init()` / `ModMain.Destroy()`.

## How it works

1. **Hook** – [Patch_EventsMgr_Init.cs](GGBH_MOD/Patch_EventsMgr_Init.cs) is a Harmony postfix on `EventsMgr.Init`. When a new `g.events` instance is detected, it (re)subscribes to the `GameCMDMelonLoader` event.
2. **Dispatch** – [ModMain.cs](GGBH_MOD/ModMain.cs) handles each `CMDMelonLoader` command:

| Command | Values | Behavior |
|---|---|---|
| `LoadDll` | `[0]` DLL path, `[1]` mod namespace | Loads the DLL via `MelonAssembly.LoadMelonAssembly` and registers it under the namespace. |
| `InitModMain` | namespaces of mods to enable | If `g.mod` changed since the last call, calls `Destroy()` on all loaded mods, then instantiates `<namespace>.ModMain` for each requested mod and calls `Init()`. |
| `DestroyModMain` | – | Recognised but currently does nothing. |

3. **Ready signal** – On `OnInitializeMelon`, an empty file `MelonLoader/GGBH_MOD_LOAD_COMPLETE.txt` is written under the game's working directory so the game side knows the loader is ready.

Mod DLLs loaded this way are expected to expose a class `<Namespace>.ModMain` with parameterless `Init()` and `Destroy()` methods (invoked via reflection).

## Project layout

```
GGBH_MOD.csproj                   Project file (net6.0)
Properties/AssemblyInfo.cs        MelonInfo / MelonGame attributes
GGBH_MOD/ModMain.cs               Melon entry point and command handling
GGBH_MOD/Patch_EventsMgr_Init.cs  Harmony patch hooking the event system
```

## Requirements

- .NET SDK with `net6.0` target support
- Tale of Immortal with [MelonLoader](https://melonwiki.xyz) **0.6.2 or newer** installed (Il2Cpp assemblies generated)

## Build

The project references game/MelonLoader assemblies by relative path (`..\..\MelonLoader\...`), so it must stay at `<GameRoot>\Sources\GGBH_MOD`:

- `MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll`
- `MelonLoader\Il2CppAssemblies\Il2Cppmscorlib.dll`
- `MelonLoader\net6\Il2CppInterop.Runtime.dll`
- `MelonLoader\net6\MelonLoader.dll`
- `MelonLoader\net6\0Harmony.dll`

```
dotnet build -c Release
```

Copy `bin\Release\net6.0\GGBH_MOD.dll` to the game's `Mods` folder.

## Notes

- Mod version: 1.0.0
- Exceptions during `LoadDll` are swallowed silently; `Init`/`Destroy` errors are printed to the console.
