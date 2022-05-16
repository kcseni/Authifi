using System;

using Authifi.Contracts.Services;
using Authifi.Core.Contracts.Services;
using Authifi.Models;
using Authifi.ViewModels;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Authifi.Tests.MSTest
{
    [TestClass]
    public class SettingsViewModelTests
    {
        public SettingsViewModelTests()
        {
        }

        [TestMethod]
        public void TestSettingsViewModel_SetCurrentTheme()
        {
            var mockThemeSelectorService = new Mock<IThemeSelectorService>();
            mockThemeSelectorService.Setup(mock => mock.GetCurrentTheme()).Returns(AppTheme.Light);
            var mockAppConfig = new Mock<AppConfig>();
            var mockSystemService = new Mock<ISystemService>();
            var mockApplicationInfoService = new Mock<IApplicationInfoService>();
            var mockUserDataService = new Mock<IUserDataService>();
            var mockIdentityService = new Mock<IIdentityService>();

            var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object, mockUserDataService.Object, mockIdentityService.Object);
            settingsVm.OnNavigatedTo(null);

            Assert.AreEqual(AppTheme.Light, settingsVm.Theme);
        }

        [TestMethod]
        public void TestSettingsViewModel_SetCurrentVersion()
        {
            var mockThemeSelectorService = new Mock<IThemeSelectorService>();
            var mockAppConfig = new Mock<AppConfig>();
            var mockSystemService = new Mock<ISystemService>();
            var mockApplicationInfoService = new Mock<IApplicationInfoService>();
            var mockUserDataService = new Mock<IUserDataService>();
            var mockIdentityService = new Mock<IIdentityService>();
            var testVersion = new Version(1, 2, 3, 4);
            mockApplicationInfoService.Setup(mock => mock.GetVersion()).Returns(testVersion);

            var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object, mockUserDataService.Object, mockIdentityService.Object);
            settingsVm.OnNavigatedTo(null);

            Assert.AreEqual($"Authifi - {testVersion}", settingsVm.VersionDescription);
        }

        [TestMethod]
        public void TestSettingsViewModel_SetThemeCommand()
        {
            var mockThemeSelectorService = new Mock<IThemeSelectorService>();
            var mockAppConfig = new Mock<AppConfig>();
            var mockSystemService = new Mock<ISystemService>();
            var mockApplicationInfoService = new Mock<IApplicationInfoService>();
            var mockUserDataService = new Mock<IUserDataService>();
            var mockIdentityService = new Mock<IIdentityService>();

            var settingsVm = new SettingsViewModel(mockAppConfig.Object, mockThemeSelectorService.Object, mockSystemService.Object, mockApplicationInfoService.Object, mockUserDataService.Object, mockIdentityService.Object);
            settingsVm.SetThemeCommand.Execute(AppTheme.Light.ToString());

            mockThemeSelectorService.Verify(mock => mock.SetTheme(AppTheme.Light));
        }
    }
}
