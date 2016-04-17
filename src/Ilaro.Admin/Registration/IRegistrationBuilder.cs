using System;

namespace Ilaro.Admin.Registration
{
    public interface IRegistrationBuilder
    {
        IRegistrationBuilder Where(Func<Type, bool> predicate);

        IRegistrationBuilder Except<T>();

        IRegistrationBuilder InNamespaceOf<T>();

        IRegistrationBuilder InNamespace(string ns);

        void Register();
    }

    public interface ICanRegisterWithAttributes
    {
        void RegisterWithAttributes();
    }

    public interface IRegisterTypes : IRegistrationBuilder, ICanRegisterWithAttributes
    {
    }

    public interface IRegisterCustomizers : IRegistrationBuilder
    {
    }
}