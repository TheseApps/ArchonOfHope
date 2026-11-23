# Archon of Hope

A .NET MAUI app with a fixed shell. Every screen it shows comes from `/content`.

## Change the app without rebuilding it
1. Edit or add a file in `/content`.
2. Push to `main`.
3. In the app, pull down to refresh.

- `content/manifest.json`: the list of screens
- `content/pages/*.md`: screen content
- `content/terms/*.md`: glossary. `[[Term Name]]` links to `terms/term-name.md`

The app loads the live copy from GitHub. If that fails, it uses the last copy saved on the device, and then the copy bundled in the app.

## Build
Open `src/ArchonOfHope.App` in Visual Studio 2022 with the .NET MAUI workload (.NET 9). Run it on an Android emulator or a device.
