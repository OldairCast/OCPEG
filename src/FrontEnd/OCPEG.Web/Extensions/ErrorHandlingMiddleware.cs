using OCPEG.Framework;

namespace OCPEG.Web.Extensions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = new { message = "Erro interno", detail = ex.Message };

                NLogManager.LogError($"{response}");
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
