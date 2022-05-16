using System.IO;
using System.Net.Http;
using System.Reflection;

using Authifi.Contracts.Services;
using Authifi.Core.Contracts.Services;
using Authifi.Core.Services;
using Authifi.Models;
using Authifi.Services;
using Authifi.ViewModels;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Prism.Regions;

using Unity;

namespace Authifi.Tests.MSTest
{
    [TestClass]
    public class PagesTests
    {
        private readonly IUnityContainer _container;

        public PagesTests()
        {
            _container = new UnityContainer();
            _container.RegisterType<IRegionManager, RegionManager>();

            // Core Services
            _container.RegisterType<IFileService, FileService>();
            _container.RegisterType<IIdentityService, IdentityService>();
            _container.RegisterType<IMicrosoftGraphService, MicrosoftGraphService>();

            // App Services
            _container.RegisterType<IThemeSelectorService, ThemeSelectorService>();
            _container.RegisterType<ISystemService, SystemService>();
            _container.RegisterType<IPersistAndRestoreService, PersistAndRestoreService>();
            _container.RegisterType<IUserDataService, UserDataService>();
            _container.RegisterType<IIdentityCacheService, IdentityCacheService>();
            _container.RegisterFactory<IHttpClientFactory>(container => GetHttpClientFactory());
            _container.RegisterType<IApplicationInfoService, ApplicationInfoService>();

            // Configuration
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .Build();
            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            _container.RegisterInstance(configuration);
            _container.RegisterInstance(appConfig);
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

        // TODO WTS: Add tests for functionality you add to KaraokeViewModel.
        [TestMethod]
        public void TestKaraokeViewModelCreation()
        {
            var vm = _container.Resolve<KaraokeViewModel>();
            Assert.IsNotNull(vm);
        }

        // TODO WTS: Add tests for functionality you add to MainViewModel.
        [TestMethod]
        public void TestMainViewModelCreation()
        {
            var vm = _container.Resolve<MainViewModel>();
            Assert.IsNotNull(vm);
        }

        // TODO WTS: Add tests for functionality you add to ProfileViewModel.
        [TestMethod]
        public void TestProfileViewModelCreation()
        {
            var vm = _container.Resolve<ProfileViewModel>();
            Assert.IsNotNull(vm);
        }

        // TODO WTS: Add tests for functionality you add to SettingsViewModel.
        [TestMethod]
        public void TestSettingsViewModelCreation()
        {
            var vm = _container.Resolve<SettingsViewModel>();
            Assert.IsNotNull(vm);
        }
    }
}
