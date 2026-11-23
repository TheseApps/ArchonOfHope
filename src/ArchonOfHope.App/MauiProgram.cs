using ArchonOfHope.Services;

namespace ArchonOfHope;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(8) });
        builder.Services.AddSingleton<ContentService>();

        return builder.Build();
    }
}
