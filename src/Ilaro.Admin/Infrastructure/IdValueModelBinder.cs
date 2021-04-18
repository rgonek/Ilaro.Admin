using Dawn;
using Ilaro.Admin.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ilaro.Admin.Infrastructure
{
    public class IdValueModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Guard.Argument(bindingContext, nameof(bindingContext)).NotNull();

            var idResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (idResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            var entityResult = bindingContext.ValueProvider.GetValue(nameof(Entity));
            if (entityResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var entities = bindingContext.HttpContext.RequestServices.GetService<IEntityCollection>();
            var entity = entities[entityResult.FirstValue];
            bindingContext.Result = ModelBindingResult.Success(entity.Id.Fill(idResult.FirstValue));

            return Task.CompletedTask;
        }
    }
}
