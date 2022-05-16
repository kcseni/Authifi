using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using Authifi.Constants;
using Authifi.Contracts.Services;
using Authifi.Core.Contracts.Services;
using Authifi.Core.Models;
using Authifi.Core.DBController;
using Authifi.Core.Services;
using Authifi.Models;
using Authifi.Services;
using Authifi.ViewModels;
using Authifi.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unity;

namespace Authifi
{
    // For more inforation about application lifecyle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
    // For docs about using Prism in WPF see https://prismlibrary.com/docs/wpf/introduction.html

    // WPF UI elements use language en-US by default.
    // If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
    // Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
    public partial class App : PrismApplication
    {
        private LogInWindow _logInWindow;

        private bool _isInitialized;

        private string[] _startUpArgs;

        

        

        public App()
        {
            this.InitializeComponent();
            
        }

        protected override Window CreateShell()
            => Container.Resolve<ShellWindow>();

        protected override async void OnInitialized()
        {
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.RestoreData();

            var themeSelectorService = Container.Resolve<IThemeSelectorService>();
            themeSelectorService.InitializeTheme();

            //var userLogged = await TryUserLogin();
            /*if (!userLogged)
            {
                return;
            }*/

            await Spotify.Authentication.InitAsync();


            base.OnInitialized();
        }

        private async Task<bool> TryUserLogin()
        {
            var userDataService = Container.Resolve<IUserDataService>();
            userDataService.Initialize();

            var identityService = Container.Resolve<IIdentityService>();
            if (!_isInitialized)
            {
                var config = Container.Resolve<AppConfig>();
                identityService.InitializeWithAadAndPersonalMsAccounts(config.IdentityClientId, "http://localhost");
            }

            identityService.LoggedIn += OnLoggedIn;
            identityService.LoggedOut += OnLoggedOut;

            var silentLoginSuccess = await identityService.AcquireTokenSilentAsync();
            if (!silentLoginSuccess || !identityService.IsAuthorized())
            {
                var loginWindow = Application.Current.Windows.OfType<LogInWindow>().FirstOrDefault();
                if (loginWindow != null)
                {
                    loginWindow.Activate();
                    loginWindow.WindowState = WindowState.Normal;
                }
                else
                {
                    ShowLogInWindow();
                    _isInitialized = true;
                }

                return false;
            }

            return true;
        }

        private void OnLoggedIn(object sender, EventArgs e)
        {
            if (!(Application.Current.MainWindow is ShellWindow))
            {
                Application.Current.MainWindow = CreateShell();
                RegionManager.UpdateRegions();
            }

            Application.Current.MainWindow.Show();
            _logInWindow.Close();
        }

        private void OnLoggedOut(object sender, EventArgs e)
        {
            ShowLogInWindow();
            Application.Current.MainWindow.Close();
        }

        private void ShowLogInWindow()
        {
            _logInWindow = Container.Resolve<LogInWindow>();
            _logInWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _startUpArgs = e.Args;
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Core Services
            containerRegistry.Register<IMicrosoftGraphService, MicrosoftGraphService>();
            containerRegistry.GetContainer().RegisterFactory<IHttpClientFactory>(container => GetHttpClientFactory());
            containerRegistry.Register<IIdentityCacheService, IdentityCacheService>();
            containerRegistry.RegisterSingleton<IIdentityService, IdentityService>();
            containerRegistry.Register<IFileService, FileService>();

            // App Services
            containerRegistry.RegisterSingleton<IUserDataService, UserDataService>();
            containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
            containerRegistry.Register<ISystemService, SystemService>();
            containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();
            containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();

            // Views
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>(PageKeys.Settings);
            containerRegistry.RegisterForNavigation<ProfilePage, ProfileViewModel>(PageKeys.Profile);
            containerRegistry.RegisterForNavigation<MainPage, MainViewModel>(PageKeys.Main);
            containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();

            // Configuration
            var configuration = BuildConfiguration();
            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            containerRegistry.RegisterInstance<IConfiguration>(configuration);
            containerRegistry.RegisterInstance<AppConfig>(appConfig);
        }

        private IHttpClientFactory GetHttpClientFactory()
        {
            var services = new ServiceCollection();
            services.AddHttpClient("msgraph", client =>
            {
                client.BaseAddress = new System.Uri("https://graph.microsoft.com/v1.0/");
            });

            return services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }

        private IConfiguration BuildConfiguration()
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            return new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .AddCommandLine(_startUpArgs)
                .Build();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Spotify.Authentication.onExit();
            var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
            persistAndRestoreService.PersistData();

        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
        }
    }
}
