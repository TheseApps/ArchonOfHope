using System.Text.RegularExpressions;
using ArchonOfHope.Services;
using Markdig;

namespace ArchonOfHope.Pages;

/// <summary>
/// Renders one markdown file. [[Term]] links open content/terms/term.md,
/// so the glossary is a navigable wiki.
/// </summary>
public partial class DocPage : ContentPage
{
    private static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private readonly ContentService _content;
    private readonly string _path;
    private readonly WebView _web = new();

    public DocPage(ContentService content, string path, string title)
    {
        _content = content;
        _path = path;
        Title = title;
        Content = _web;
        _web.Navigating += OnNavigating;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var md = await _content.GetTextAsync(_path);
        md = WikiLink().Replace(md, m =>
        {
            var target = m.Groups[1].Value.Trim();
            var label = m.Groups[3].Success ? m.Groups[3].Value : target;
            return $"[{label}](app://terms/{Slug(target)}.md)";
        });

        var body = Markdown.ToHtml(md, Pipeline);
        _web.Source = new HtmlWebViewSource { Html = Wrap(body) };
    }

    private async void OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        var url = e.Url ?? "";
        if (url.StartsWith("app://"))
        {
            e.Cancel = true;
            var path = url["app://".Length..];
            await Navigation.PushAsync(new DocPage(_content, path, Path.GetFileNameWithoutExtension(path).Replace('-', ' ')));
        }
        else if (url.StartsWith("http"))
        {
            e.Cancel = true;
            await Launcher.OpenAsync(url);
        }
    }

    public static string Slug(string s) =>
        Regex.Replace(s.ToLowerInvariant().Trim(), @"[^a-z0-9]+", "-").Trim('-');

    [GeneratedRegex(@"\[\[([^\]|]+)(\|([^\]]+))?\]\]")]
    private static partial Regex WikiLink();

    private static string Wrap(string body) => $$"""
        <!DOCTYPE html><html><head>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <style>
          body { font-family: -apple-system, Roboto, sans-serif; line-height: 1.55; padding: 16px 20px; margin: 0; }
          a { color: #3b82f6; }
          img { max-width: 100%; }
          @media (prefers-color-scheme: dark) { body { background:#111; color:#e5e5e5; } a { color:#60a5fa; } }
        </style></head><body>{{body}}</body></html>
        """;
}
