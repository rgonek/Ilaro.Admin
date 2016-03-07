using System;
using System.Collections.Generic;
using Ilaro.Admin.Extensions;

namespace Ilaro.Admin.Registration
{
    public class RegistrationBuilder
    {
        /// <summary>
        /// The filters applied to the types from the scanned assembly.
        /// </summary>
        public ICollection<Func<Type, bool>> Filters { get; } = new List<Func<Type, bool>>();

        private ICollection<Action> _configurationCallbacks = new List<Action>();

        internal void RegisterCallback(Action configurationCallback)
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
            foreach(var callback in _configurationCallbacks)
            {
                callback();
            }
        }
    }
}