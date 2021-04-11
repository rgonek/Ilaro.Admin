using System;

namespace Ilaro.Admin.Core.Registration
{
    public interface IRegistrationBuilder
    {
        IRegistrationBuilder Where(Func<Type, bool> predicate);

        IRegistrationBuilder Except<T>();

        IRegistrationBuilder InNamespaceOf<T>();

        IRegistrationBuilder InNamespace(string ns);

        void Register();
    }

    public interface IRegisterTypes : IRegistrationBuilder
    {
    }

    public interface IRegisterCustomizers : IRegistrationBuilder
    {
    }
}