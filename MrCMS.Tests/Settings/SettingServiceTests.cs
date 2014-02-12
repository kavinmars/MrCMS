using System;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Settings;
using MrCMS.Settings;
using Xunit;
using FluentAssertions;
using MrCMS.Helpers;

namespace MrCMS.Tests.Settings
{
    public class SettingServiceTests : InMemoryDatabaseTest
    {
        private readonly SettingService _settingService;

        public SettingServiceTests()
        {
            _settingService = new SettingService(Session, CurrentSite);
            _settingService.ResetSettingCache();
        }
        [Fact]
        public void SettingService_GetSettingById_LoadsFromSession()
        {
            var setting = new Setting();
            Session.Transact(session => session.Add(setting));

            var settingById = _settingService.GetSettingById(1);

            settingById.Should().Be(setting);
        }

        [Fact]
        public void SettingService_DeleteSetting_CallsSessionDelete()
        {
            var setting = new Setting();
            Session.Transact(session => session.Add(setting));

            _settingService.DeleteSetting(setting);

            Session.Query<Setting>().Count().Should().Be(0);
        }

        [Fact]
        public void SettingService_DeleteSetting_NullSettingThrowsArgumentNullException()
        {
            this.Invoking(tests => _settingService.DeleteSetting(null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_SetSetting_AddsANewSettingToTheSession()
        {
            var site = new Site();
            Session.Transact(session => session.Add(site));

            _settingService.SetSetting("test", "value");

            Session.Query<Setting>().Should().HaveCount(1);
        }

        [Fact]
        public void SettingService_SetSettingShouldUpdateExistingSetting()
        {
            Session.Transact(session => session.Add(new Setting { Name = "test", Value = "value", Site = CurrentSite }));
            _settingService.SetSetting("test", "value2");

            var settings = Session.Query<Setting>().ToList();

            settings.Should().HaveCount(1);
            settings[0].Name.Should().Be("test");
            settings[0].Value.Should().Be("value2");
        }

        [Fact]
        public void SettingService_SetSetting_IfTheKeyIsNullThrowArgumentNullException()
        {
            this.Invoking(tests => _settingService.SetSetting(null, "value")).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfKeyDoesNotExist()
        {
            _settingService.GetSettingById(-1).Should().BeNull();
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsTheSettingsObjectWithTheValidKey()
        {
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Add(setting1));
            var setting2 = new Setting { Name = "test2", Value = "value2", Site = CurrentSite };
            Session.Transact(session => session.Add(setting2));

            _settingService.GetSettingByKey("test2").Should().Be(setting2);
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfKeyIsNull()
        {
            _settingService.GetSettingByKey(null).Should().Be(null);
        }

        [Fact]
        public void SettingService_GetSettingByKey_ReturnsNullIfTheKeyDoesNotExist()
        {
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Add(setting1));

            _settingService.GetSettingByKey("test2").Should().Be(null);
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsDefaultForNullKey()
        {
            _settingService.GetSettingValueByKey(null, "default").Should().Be("default");
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_ReturnsValueForSetting()
        {
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Add(setting1));
            var setting2 = new Setting { Name = "test2", Value = "value2", Site = CurrentSite };
            Session.Transact(session => session.Add(setting2));

            _settingService.GetSettingValueByKey("test2", "default").Should().Be("value2");
        }

        [Fact]
        public void SettingService_GetSettingValueByKey_DefaultWhenKeyDoesNotExist()
        {
            var setting1 = new Setting { Name = "test", Value = "value", Site = CurrentSite };
            Session.Transact(session => session.Add(setting1));

            _settingService.GetSettingValueByKey("test2", "default").Should().Be("default");
        }
    }
}