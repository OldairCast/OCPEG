using System.Globalization;

namespace OCPEG.API.Middleware
{
    /// <summary>
    /// Classe responsável pela troca de cultura
    /// </summary>
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="next"></param>
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Seta a cultura
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            
            //Se for utilizar somente as culturas de .resx
            var supportedLanguagesLocal = new List<CultureInfo> { new CultureInfo("pt-BR"), new CultureInfo("en"), new CultureInfo("es") };

            var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

            var cultureInfo = new CultureInfo("pt-BR");

            if (!string.IsNullOrEmpty(requestedCulture)
                && supportedLanguages.Exists(c => c.Name.Equals(requestedCulture)))
            {
                cultureInfo = new CultureInfo(requestedCulture);
            }

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            await _next(context);
        }
    }

}
