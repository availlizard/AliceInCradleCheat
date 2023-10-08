# AliceInCradleCheat

A BepInEx based cheat plugin for AliceInCradle.

## Building

Note: path to the project folder should not contain any non-ASCII characters.

### Obtaining dependencies

Install `Alice In Cradle v0.24`: download and unpack released package, then copy `Assembly-CSharp.dll`, `better.dll`, `netstandard.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.dll`, `UnityEngine.UI.dll` and `unsafeAssem.dll` to the lib folder from the `AliceInCradle_Data\Managed` folder.

### Building Binaries

1. Open `AliceInCradleCheat.sln` and let Visual Studio load everything. Install dependencies via NuGet if prompted.
2. In the toolbar, select the desired Configuration (`Debug` or `Release`).
3. From the Build menu, select `Build Solution` (or use the keybind; default is `F7`)
4. Copy the `AliceInCradleCheat.dll` from within the `bin` folder to the plugins folder within BepInEx in the game folder.
