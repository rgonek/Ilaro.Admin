using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Core
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly Func<NameValueCollection> _data;
        private readonly ValueConverter _converter = new ValueConverter();

        public ConfigurationProvider()
        {
            _data = () => ConfigurationManager.AppSettings;
        }

        public bool IsConfigured(string key)
        {
            return _data().AllKeys.ToList().Contains(key);
        }

        public TOutput Get<TOutput>(string key)
        {
            var value = _data()[key];
            if (value == null)
            {
                throw new ConfigurationErrorsException("Configuration entry '{0}' is missing.".Fill(key));
            }
            return _converter.GetAs<TOutput>(value);
        }

        public TOutput Get<TOutput>(string key, TOutput defaultValue)
        {
            var value = _data()[key];
            if (value == null)
            {
                return defaultValue;
            }

            return _converter.GetAs<TOutput>(value);
        }
    }
}