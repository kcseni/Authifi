using System;
using System.Windows.Input;

using Authifi.Contracts.Services;
using Authifi.Core.Contracts.Services;
using Authifi.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Authifi.ViewModels
{
    // TODO WTS: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
    public class SettingsViewModel : BindableBase, INavigationAware
    {
        private readonly AppConfig _appConfig;
        private readonly IUserDataService _userDataService;
        private readonly IIdentityService _identityService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private AppTheme _theme;
        private string _versionDescription;
        private UserViewModel _user;
        private ICommand _setThemeCommand;
        private ICommand _privacyStatementCommand;
        private ICommand _logOutCommand;

        public AppTheme Theme
        {
            get { return _theme; }
            set { SetProperty(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return _versionDescription; }
            set { SetProperty(ref _versionDescription, value); }
        }

        public UserViewModel User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new DelegateCommand<string>(OnSetTheme));

        public ICommand PrivacyStatementCommand => _privacyStatementCommand ?? (_privacyStatementCommand = new DelegateCommand(OnPrivacyStatement));

        public ICommand LogOutCommand => _logOutCommand ?? (_logOutCommand = new DelegateCommand(OnLogOut));

        public SettingsViewModel(AppConfig appConfig, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService, IUserDataService userDataService, IIdentityService identityService)
        {
            _appConfig = appConfig;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            _userDataService = userDataService;
            _identityService = identityService;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
            Theme = _themeSelectorService.GetCurrentTheme();
            _identityService.LoggedOut += OnLoggedOut;
            _userDataService.UserDataUpdated += OnUserDataUpdated;
            User = _userDataService.GetUser();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnregisterEvents();
        }

        private void UnregisterEvents()
        {
            _identityService.LoggedOut -= OnLoggedOut;
            _userDataService.UserDataUpdated -= OnUserDataUpdated;
        }

        private void OnSetTheme(string themeName)
        {
            var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
            _themeSelectorService.SetTheme(theme);
        }

        private void OnPrivacyStatement()
            => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

        public bool IsNavigationTarget(NavigationContext navigationContext)
            => true;

        private async void OnLogOut()
        {
            await _identityService.LogoutAsync();
        }

        private void OnUserDataUpdated(object sender, UserViewModel userData)
        {
            User = userData;
        }

        private void OnLoggedOut(object sender, EventArgs e)
        {
            UnregisterEvents();
        }
    }
}
