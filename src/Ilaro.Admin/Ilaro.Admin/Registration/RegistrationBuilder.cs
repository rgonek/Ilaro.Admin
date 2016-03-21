using System;
using System.Collections.Generic;
using Ilaro.Admin.Extensions;
using Ilaro.Admin.Configuration.Customizers;
using Ilaro.Admin.Configuration;

namespace Ilaro.Admin.Registration
{
    public class RegistrationBuilder
    {
        /// <summary>
        /// The filters applied to the types from the scanned assembly.
        /// </summary>
        public ICollection<Func<Type, bool>> Filters { get; } = new List<Func<Type, bool>>();

        private ICollection<Action<Action<ICustomizersHolder>>> _configurationCallbacks = new List<Action<Action<ICustomizersHolder>>>();

        internal void RegisterCallback(Action<Action<ICustomizersHolder>> configurationCallback)
        {
            _configurationCallbacks.Add(configurationCallback);
        }

        public RegistrationBuilder Where(Func<Type, bool> predicate)
        {
            Filters.Add(predicate);
            return this;
        }

        public RegistrationBuilder Except<T>()
        {
            return Where(t => t != typeof(T));
        }

        public RegistrationBuilder InNamespaceOf<T>()
        {
            return InNamespace(typeof(T).Namespace);
        }

        public RegistrationBuilder InNamespace(string ns)
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

        public void RegisterWithAttributes()
        {
            foreach (var callback in _configurationCallbacks)
            {
                callback(customizer => { AttributesConfigurator.Initialise(customizer); });
            }
        }
    }
}