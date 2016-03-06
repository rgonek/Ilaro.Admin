using System;
using System.Collections.Generic;

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

        public void Register()
        {
            foreach(var callback in _configurationCallbacks)
            {
                callback();
            }
        }
    }
}