using ArchonOfHope.Services;

namespace ArchonOfHope.Pages;

/// <summary>Lists the screens defined in content/manifest.json.</summary>
public class HomePage : ContentPage
{
    private readonly ContentService _content;
    private readonly CollectionView _list;
    private readonly RefreshView _refresh;

    public HomePage(ContentService content)
    {
        _content = content;
        Title = "Archon of Hope";

        _list = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            ItemTemplate = new DataTemplate(() =>
            {
                var title = new Label { FontSize = 20, FontAttributes = FontAttributes.Bold };
                title.SetBinding(Label.TextProperty, nameof(Screen.Title));
                var sub = new Label { FontSize = 14, Opacity = 0.7 };
                sub.SetBinding(Label.TextProperty, nameof(Screen.Subtitle));
                return new VerticalStackLayout { Padding = new Thickness(20, 14), Children = { title, sub } };
            })
        };
        _list.SelectionChanged += async (_, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is Screen s)
            {
                _list.SelectedItem = null;
                await Navigation.PushAsync(new DocPage(_content, s.Page, s.Title));
            }
        };

        _refresh = new RefreshView { Content = _list };
        _refresh.Refreshing += async (_, _) =>
        {
            _content.ClearMemory();
            await LoadAsync();
            _refresh.IsRefreshing = false;
        };

        Content = _refresh;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var m = await _content.GetManifestAsync();
        Title = m.Title;
        _list.ItemsSource = m.Screens;
    }
}
