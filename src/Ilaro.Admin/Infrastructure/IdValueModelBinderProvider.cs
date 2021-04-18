using Dawn;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ilaro.Admin.Infrastructure
{
    public class IdValueModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            Guard.Argument(context, nameof(context)).NotNull();

            var modelType = context.Metadata.UnderlyingOrModelType;
            if (modelType == typeof(IdValue))
            {
                return new IdValueModelBinder();
            }

            return null;
        }
    }
}
