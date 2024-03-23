using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using ReactiveUI;
using System;
using Material.Styles; // Correct namespace for Material.Avalonia

namespace QuickEndpoint.ViewModels
{
    public class OpenSettingsViewModel : ViewModelBase
    {
        private bool _isDarkThemeEnabled;

        // Property to track if dark theme is enabled
        public bool IsDarkThemeEnabled
        {
            get => _isDarkThemeEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _isDarkThemeEnabled, value);
                ApplyTheme(value);
            }
        }

        private void ApplyTheme(bool useDarkTheme)
        {
            // Construct the URI for the theme resource
            var themeUri = useDarkTheme ? 
                "avares://QuickEndpoint/Themes/DarkTheme.axaml" :
                "avares://QuickEndpoint/Themes/LightTheme.axaml";

            var theme = new StyleInclude(new Uri(themeUri))
            {
                Source = new Uri(themeUri)
            };

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Assuming there is a main window and its styles can be manipulated
                desktop.MainWindow.Styles.Clear();
                desktop.MainWindow.Styles.Add(theme);
            }
        }
    }
}
