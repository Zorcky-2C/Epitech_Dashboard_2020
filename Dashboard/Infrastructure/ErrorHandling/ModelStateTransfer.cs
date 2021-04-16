using Microsoft.AspNetCore.Mvc.Filters;

namespace Dashboard.Infrastructure.ErrorHandling
{
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected const string Key = nameof(ModelStateTransfer);
    }
}