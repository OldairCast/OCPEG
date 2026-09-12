using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using static OCPEG.Domain.Enum.Enums;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class UserController : BaseController
    {

        private readonly IAccessControlHost _accessControlClient;
        private readonly UserViewModel model = new UserViewModel();
        private const string urlEdit = "~/Views/User/Edit.cshtml";
        public UserController(IAccessControlHost accessControl)
        {
            _accessControlClient = accessControl ?? throw new ArgumentNullException(nameof(accessControl));
        }


        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            string token = GetToken();

            string userIdentifier = HttpContext.Session.GetString("UserIdentifier") ?? string.Empty;

            var objJsonResponse = await _accessControlClient.GetUserIdentifierLogged(token, userIdentifier);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                var lstUser = new List<UserDto>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/User/Index.cshtml", lstUser);
            }

            UserDto userDto = objJsonResponse.Exchange;

            if (userDto.Admin)
            {
                var objJsonResponseAll = await _accessControlClient.GetUserAll(token);

                if (objJsonResponseAll.Status != ResponseResultStatus.Success)
                {
                    var lstUser = new List<UserDto>();
                    ModelState.AddModelError(string.Empty, objJsonResponseAll.Message);
                    return View("~/Views/User/Index.cshtml", lstUser);
                }

                List<UserDto> users = objJsonResponseAll.Exchange;

                return View("~/Views/User/Index.cshtml", users);

            }
            else
            {
                return View(urlEdit, userDto);
            }
        }

        public ActionResult Create()
        {
            return View("~/Views/User/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto user)
        {
            string token = GetToken();

            var objJsonResponse = await _accessControlClient.CreateUser(token, user);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            List<ComboOption>? Role = new List<ComboOption>();
            List<ComboOption>? FunctionF = new List<ComboOption>();

            string token = GetToken();

            //Obtém o cliente
            var objJsonResponse = await _accessControlClient.GetUserById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlEdit, model);
            }

            foreach (UserRolesEn role in Enum.GetValues(typeof(UserRolesEn)))
            {
                byte idRole = (byte)role;

                if (idRole != 0)
                {
                    ComboOption userRole = new ComboOption
                    {
                        Id = idRole.ToString(),
                        Name = GetDescription(role)
                    };

                    Role.Add(userRole);
                }
            }


            foreach (FunctionEn function in Enum.GetValues(typeof(FunctionEn)))
            {
                byte idF = (byte)function;
                ComboOption userRole = new ComboOption
                {
                    Id = idF.ToString(),
                    Name = GetDescription(function)
                };

                FunctionF.Add(userRole);
            }

            model.User = objJsonResponse.Exchange;
            model.Role = Role;
            model.Function = FunctionF;
            model.FunctionId = model.User.Function.ToString();
            model.RoleId = model.User.Admin ? Convert.ToByte(UserRolesEn.Admin).ToString() : Convert.ToByte(UserRolesEn.User).ToString();

            return View(urlEdit, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserDto user)
        {
            string token = GetToken();

            if (user.ProfileId == "1")
            {
                user.Admin = true;
            }

            var objJsonResponse = await _accessControlClient.UpdateUser(token, user);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.User = new UserDto();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlEdit, model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            string token = GetToken();

            var objJsonResponse = await _accessControlClient.GetUserById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                UserDto user = new UserDto();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(user);
            }

            UserDto userData = objJsonResponse.Exchange;

            return View(userData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, IFormCollection collection)
        {
            string token = GetToken();

            var objJsonResponse = await _accessControlClient.DeleteUserById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                UserDto user = new UserDto();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(user);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
