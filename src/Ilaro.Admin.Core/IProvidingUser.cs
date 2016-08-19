namespace Ilaro.Admin.Core
{
    public interface IProvidingUser
    {
        string CurrentUserName();
        object CurrentId();
    }
}