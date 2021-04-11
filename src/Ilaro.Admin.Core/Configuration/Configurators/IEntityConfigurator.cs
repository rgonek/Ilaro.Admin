namespace Ilaro.Admin.Core.Configuration.Configurators
{
    public interface IEntityConfigurator
    {
        IConfiguratorsHolder CustomizersHolder { get; }
    }
}