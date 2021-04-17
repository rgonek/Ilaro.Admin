using Dawn;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ilaro.Admin.Infrastructure
{
    public class EntityModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Guard.Argument(bindingContext, nameof(bindingContext)).NotNull();

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var entities = bindingContext.HttpContext.RequestServices.GetService<IEntityCollection>();
            var entity = entities[valueProviderResult.FirstValue];
            bindingContext.Result = ModelBindingResult.Success(entity);

            return Task.CompletedTask;
        }
    }
}
