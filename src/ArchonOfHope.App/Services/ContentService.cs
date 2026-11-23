using System.Text.Json;

namespace ArchonOfHope.Services;

public record Screen(string Id, string Title, string Page, string? Subtitle);
public record Manifest(string Title, int Version, List<Screen> Screens);

/// <summary>
/// Loads content with a three-step fallback:
///   1. live from GitHub (edit a .md file, push, app updates)
///   2. last copy cached on the device
///   3. copy bundled into the app at build time
/// </summary>
public class ContentService
{
    public const string RemoteBase =
        "https://raw.githubusercontent.com/TheseApps/ArchonOfHope/main/content/";

    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly Dictionary<string, string> _memory = new();

    public ContentService(HttpClient http) => _http = http;

    public void ClearMemory() => _memory.Clear();

    public async Task<Manifest> GetManifestAsync()
    {
        var text = await GetTextAsync("manifest.json");
        return JsonSerializer.Deserialize<Manifest>(text, Json)
               ?? new Manifest("Archon of Hope", 0, new());
    }

    public async Task<string> GetTextAsync(string path)
    {
        path = path.TrimStart('/');
        if (_memory.TryGetValue(path, out var hit)) return hit;

        var cacheFile = Path.Combine(FileSystem.AppDataDirectory, "content", path);

        string? text = null;
        try
        {
            text = await _http.GetStringAsync(RemoteBase + path);
            Directory.CreateDirectory(Path.GetDirectoryName(cacheFile)!);
            await File.WriteAllTextAsync(cacheFile, text);
        }
        catch
        {
            if (File.Exists(cacheFile))
                text = await File.ReadAllTextAsync(cacheFile);
            else
            {
                try
                {
                    using var s = await FileSystem.OpenAppPackageFileAsync("content/" + path);
                    using var r = new StreamReader(s);
                    text = await r.ReadToEndAsync();
                }
                catch { /* not bundled */ }
            }
        }

        text ??= $"# Not found\n\n`{path}` isn't written yet.";
        _memory[path] = text;
        return text;
    }
}
