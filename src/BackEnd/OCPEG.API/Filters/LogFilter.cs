using Microsoft.AspNetCore.Mvc.Filters;
using OCPEG.Framework;

namespace OCPEG.API.Filters
{
    /// <summary>
    /// Classe de Filtro de Log
    /// </summary>
    public class LogFilter : IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        public LogFilter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            NLogManager.LogInfoX($"{context.RouteData.Values["controller"]}", $"{context.RouteData.Values["action"]}", Constants.Inicio);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code after action executes
            NLogManager.LogInfoX($"{context.RouteData.Values["controller"]}", $"{context.RouteData.Values["action"]}", Constants.Fim);
        }

    }
}
