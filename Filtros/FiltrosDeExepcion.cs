using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filtros
{
    public class FiltrosDeExepcion : ExceptionFilterAttribute
    {
        public ILogger<FiltrosDeExepcion> logger { get; }

        public FiltrosDeExepcion(ILogger<FiltrosDeExepcion> logger) {
           this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
