using ArchonOfHope.Pages;
using ArchonOfHope.Services;

namespace ArchonOfHope;

/// <summary>
/// The app shell never changes. Everything the user sees comes from /content.
/// </summary>
public class App : Application
{
    private readonly ContentService _content;

    public App(ContentService content) => _content = content;

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new NavigationPage(new HomePage(_content)));
}
