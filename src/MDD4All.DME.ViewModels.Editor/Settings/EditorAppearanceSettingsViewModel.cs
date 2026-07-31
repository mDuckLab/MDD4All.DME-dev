using CommunityToolkit.Mvvm.ComponentModel;
using MDD4All.Configuration;
using MDD4All.Configuration.Contracts;

namespace MDD4All.DME.ViewModels.Editor.Settings
{
    public class EditorAppearanceSettingsViewModel : ObservableObject
    {
        private readonly IConfigurationReaderWriter<EditorAppearanceSettings> _configurationReaderWriter;

        private EditorAppearanceSettings _settings;

        public EditorAppearanceSettingsViewModel()
        {
            _configurationReaderWriter = new FileConfigurationReaderWriter<EditorAppearanceSettings>("DME");

            _settings = _configurationReaderWriter.GetConfiguration() ?? new EditorAppearanceSettings();
        }

        public bool TintEnabled
        {
            get => _settings.TintEnabled;
            set => SetAndStore(value, _settings.TintEnabled, v => _settings.TintEnabled = v);
        }

        public int MaxDepth
        {
            get => _settings.MaxDepth;
            set => SetAndStore(value, _settings.MaxDepth, v => _settings.MaxDepth = v);
        }

        public bool ShowIcons
        {
            get => _settings.ShowIcons;
            set => SetAndStore(value, _settings.ShowIcons, v => _settings.ShowIcons = v);
        }

        public bool ShowIndexNumbers
        {
            get => _settings.ShowIndexNumbers;
            set => SetAndStore(value, _settings.ShowIndexNumbers, v => _settings.ShowIndexNumbers = v);
        }

        private void SetAndStore<T>(T value, T currentValue, System.Action<T> apply, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(currentValue, value))
            {
                apply(value);
                _configurationReaderWriter.StoreConfiguration(_settings);
                OnPropertyChanged(propertyName);
            }
        }
    }
}
