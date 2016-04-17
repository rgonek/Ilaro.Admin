namespace Ilaro.Admin.Core
{
    public interface IConfigurationProvider
    {
        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);
        bool IsConfigured(string key);
    }
}
