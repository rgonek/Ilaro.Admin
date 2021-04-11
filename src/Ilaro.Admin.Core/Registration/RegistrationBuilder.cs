using System;
using System.Collections.Generic;
using Ilaro.Admin.Core.Extensions;
using Ilaro.Admin.Core.Configuration.Configurators;
using Ilaro.Admin.Core.Configuration;

namespace Ilaro.Admin.Core.Registration
{
    public class RegistrationBuilder : IRegisterTypes, IRegisterCustomizers
    {
        /// <summary>
        /// The filters applied to the types from the scanned assembly.
        /// </summary>
        public ICollection<Func<Type, bool>> Filters { get; } = new List<Func<Type, bool>>();

        private ICollection<Action<Action<IConfiguratorsHolder>>> _configurationCallbacks = new List<Action<Action<IConfiguratorsHolder>>>();

        internal void RegisterCallback(Action<Action<IConfiguratorsHolder>> configurationCallback)
        {
            _configurationCallbacks.Add(configurationCallback);
        }

        public IRegistrationBuilder Where(Func<Type, bool> predicate)
        {
            Filters.Add(predicate);
            return this;
        }

        public IRegistrationBuilder Except<T>()
        {
            return Where(t => t != typeof(T));
        }

        public IRegistrationBuilder InNamespaceOf<T>()
        {
            return InNamespace(typeof(T).Namespace);
        }

        public IRegistrationBuilder InNamespace(string ns)
        {
            if (ns == null) throw new ArgumentNullException(nameof(ns));

            return Where(t => t.IsInNamespace(ns));
        }

        public void Register()
        {
            foreach (var callback in _configurationCallbacks)
            {
                callback(customizer => { });
            }
        }
    }
}