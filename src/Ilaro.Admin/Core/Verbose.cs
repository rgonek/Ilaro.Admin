using Ilaro.Admin.Extensions;
using Resources;
using System;

namespace Ilaro.Admin.Core
{
    public class Verbose
    {
        public string Singular { get; internal set; }
        public string Plural { get; internal set; }
        public string Group { get; internal set; }

        public Verbose(Type type)
        {
            Singular = type.Name.SplitCamelCase();
            Plural = Singular.Pluralize().SplitCamelCase();
            Group = IlaroAdminResources.Others;
        }
    }
}