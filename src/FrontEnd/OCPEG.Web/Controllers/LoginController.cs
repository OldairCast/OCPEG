using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using System.Security.Claims;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class LoginController : Controller
    {

        private readonly IAccessControlHost _accessControlClient;
        private readonly LoginViewModel model = new LoginViewModel();
        private readonly UserRegisterViewModel modelR = new UserRegisterViewModel();

        public LoginController(IAccessControlHost accessControl)
        {
            _accessControlClient = accessControl ?? throw new ArgumentNullException(nameof(accessControl));
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Index(UserLoginDto userLogin)
        {
            // Verifica validação dos campos e retorna os erros
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var objJsonResponse = await _accessControlClient.Login(userLogin);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View();
            }

            MakeSession(objJsonResponse.Exchange);

            return Redirect("OcPeg");
        }

        [HttpGet]
        public async Task<IActionResult> LoginGoogle()
        {
            var authenticate = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticate.Succeeded || authenticate.Principal is null
                || !authenticate.Principal.Identities.Any(id => id.IsAuthenticated))
            {
                return Challenge(GoogleDefaults.AuthenticationScheme);
            }
            else
            {
                var claims = authenticate.Principal!.Identities.First().Claims;

                var name = claims.First(c => c.Type == ClaimTypes.Name).Value;
                var email = claims.First(c => c.Type == ClaimTypes.Email).Value;

                UserDto userLogin = new UserDto
                {
                    Email = email,
                    Name = name
                };

                var objJsonResponse = await _accessControlClient.LoginGoogle(userLogin);

                if (objJsonResponse.Status != ResponseResultStatus.Success)
                {
                    ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                    return View("~/Views/Login/Index.cshtml", model);
                }

                MakeSession(objJsonResponse.Exchange);

                return Redirect("OcPeg");
            }
        }

        /// <summary>
        /// Armazena as variáveis de sessão
        /// </summary>
        /// <param name="tokenExchange"></param>
        private void MakeSession(TokenOut tokenExchange)
        {
            string token = tokenExchange.AccessToken;
            string userName = tokenExchange.UserName;
            string userIdentifier = tokenExchange.UserIdentifier;
            string refreshToken = tokenExchange.RefreshToken;
            DateTime tokenExpiryTime = tokenExchange.RefreshTokenExpiryTime;

            HttpContext.Session.SetString("UserName", userName);
            HttpContext.Session.SetString("UserIdentifier", userIdentifier);
            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("RefreshToken", refreshToken);
            HttpContext.Session.SetString("RefreshTokenExpiryTime", tokenExpiryTime.ToString());
        }


        public IActionResult Register()
        {
            return View("~/Views/Login/Register.cshtml", modelR);
        }


        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel userRegister)
        {
            // Verifica validação dos campos e retorna os erros
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            UserDto userDto = new UserDto();

            var objJsonResponse = await _accessControlClient.Register(userDto);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/Login/Register.cshtml", userRegister);
            }

            return View("Index");
        }
    }
}
