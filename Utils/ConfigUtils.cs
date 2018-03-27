using System;
using System.Configuration;

namespace Utils
{
    public static class ConfigUtils
    {
        public static int GetInt(string name, int defaultValue = 0)
        {
            string item;
            try
            {
                item = ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return defaultValue;
            }

            var value = defaultValue;
            if (int.TryParse(item, out value))
                return value;
            else
                return defaultValue;
        }

        public static double GetDouble(string name, double defaultValue = 0)
        {
            string item;
            try
            {
                item = ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return defaultValue;
            }

            var value = defaultValue;
            if (double.TryParse(item, out value))
                return value;
            else
                return defaultValue;
        }

        public static string GetString(string name, string defaultValue = null)
        {
            string item;
            try
            {
                item = ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return defaultValue;
            }

            if (item == null)
                return defaultValue;
            else
                return item;
        }

        public static T GetEnum<T>(string name, T defaultValue)
            where T: struct, IConvertible
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            string item;
            try
            {
                item = ConfigurationManager.AppSettings[name];
                if (item == null)
                    return defaultValue;
                return (T)Enum.Parse(type, item, true);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool GetBool(string name, bool defaultValue = false)
        {
            string item;
            try
            {
                item = ConfigurationManager.AppSettings[name];
            }
            catch
            {
                return defaultValue;
            }

            var value = defaultValue;
            if (bool.TryParse(item, out value))
                return value;
            else
                return defaultValue;
        }
    }
}
