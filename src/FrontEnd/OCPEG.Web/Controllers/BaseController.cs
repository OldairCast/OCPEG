using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Reflection;

namespace OCPEG.Web.Controllers
{
    public class BaseController : Controller
    {
        public string GetToken()
        {
            string? token = HttpContext.Session.GetString("Token") == null ? "" : HttpContext.Session.GetString("Token");
            return token ?? "";
        }

        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return value.ToString();

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}
