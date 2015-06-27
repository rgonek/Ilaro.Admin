using System;
using System.Linq;
using Ilaro.Admin.DataAnnotations;
using Ilaro.Admin.Extensions;
using Resources;

namespace Ilaro.Admin.Core
{
    public class Verbose
    {
        public string Singular { get; internal set; }
        public string Plural { get; internal set; }
        public string Group { get; internal set; }

        public Verbose(Type type)
        {
            var verboseAttributes = type.GetCustomAttributes(
                typeof(VerboseAttribute),
                false) as VerboseAttribute[];
            if (verboseAttributes.IsNullOrEmpty<VerboseAttribute>() == false)
            {
                var verbose = verboseAttributes.FirstOrDefault();
                Singular = verbose.Singular ?? type.Name.SplitCamelCase();
                Plural = verbose.Plural ?? Singular.Pluralize().SplitCamelCase();
                Group = verbose.GroupName ?? IlaroAdminResources.Others;
            }
            else
            {
                Singular = type.Name.SplitCamelCase();
                Plural = Singular.Pluralize().SplitCamelCase();
                Group = IlaroAdminResources.Others;
            }
        }
    }
}