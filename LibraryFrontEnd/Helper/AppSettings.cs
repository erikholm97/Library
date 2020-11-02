using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryFrontEnd.Helper
{
    public static class AppSettings
    {
        /// <summary>
        /// Returns the sortorder stored in app settings.
        /// </summary>
        /// <returns></returns>
        public static string ReadSetting()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                return appSettings[0];

            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            return string.Empty;
        }
        /// <summary>
        /// Method to update appsettings to persist the sorting during applications lifetime.
        /// </summary>
        /// <param name="value"></param>
        public static void UpdateAppSettings(string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                settings["sortSetting"].Value = value;

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
