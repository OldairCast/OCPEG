using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.BusinessObject;
using OCPEG.Framework;
using OCPEG.Framework.Exception;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CloudMessageService : ICloudMessageService
    {
        private readonly ICloudSendMessageService _messageService;
        private readonly IUserService _userService;

        public CloudMessageService(
            ICloudSendMessageService messageService,
            IUserService userService,
            IAccountService accountService)
        {
            _messageService = messageService;
            _userService = userService;
        }


        /// <summary>
        /// Método fake para funcionar a execução sem acesso ao Azure (DeleteUser)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserNoAzure(int userId)
        {
            try
            {
                User user = await _userService.GetById(userId);
                user.Active = false;

                await _userService.Update(user);
                return true;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }



        /// <summary>
        /// Deleta usuário no banco e no Azure
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                User user = await _userService.GetById(userId);
                user.Active = false;

                await _userService.Update(user);

                await _messageService.SendMessage(user);

                return true;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

    }
}
