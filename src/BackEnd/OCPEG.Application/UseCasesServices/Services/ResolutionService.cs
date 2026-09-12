using OCPEG.Application.Communication.Requests;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.BusinessObject.Message;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class ResolutionService: IResolutionService
    {
        private readonly IResolutionRepository _resolutionRepository;
        private readonly IUserRepository _userEfDAO;
        private readonly IAccountService _accountService;
        private readonly ICallService _callService;
        private readonly ICustomerService _customerService;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public ResolutionService(IResolutionRepository resolutionRepository, IUserRepository userDAO, IAccountService accountService
            , ICallService callService, ICustomerService customerService, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _resolutionRepository = resolutionRepository;
            _userEfDAO = userDAO;
            _accountService = accountService;
            _callService = callService;
            _customerService = customerService;
            _rabbitMQMessageSender = rabbitMQMessageSender;
        }

        /// <summary>
        /// Obtém dados da resolução por numero do protocolo
        /// </summary>
        /// <param name="callNumber">Numero do Protocolo</param>
        /// <returns>Objeto de negocio ResolutionDto</returns>
        public async Task<Resolution> GetByCallNumber(string callNumber)
        {
            try
            {
                Resolution resolution = await _resolutionRepository.GetByCallNumber(callNumber);

                if (resolution != null && resolution.User != null && !string.IsNullOrEmpty(resolution.User.Id))
                {
                    var user = await _userEfDAO.GetById(Convert.ToInt32(resolution.User.Id));

                    resolution.User.Name = user.Name;
                }
                else
                {
                    Resolution res = new Resolution();
                    resolution = res;
                }
                return resolution;

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
        /// Realiza a resolução de um atendimento
        /// </summary>
        ///<param name="resolution">Dados da Resolução</param>
        /// <returns>Flafg Sucesso/erro</returns>
        public async Task<bool> Insert(Resolution resolution)
        {
            try
            {
                Validate(resolution);

                var objUser = await _accountService.GetUserLogged();
                var user = objUser;
                resolution.Date = DateTime.Now;
                resolution.User = new ComboOption
                {
                    Id = user.Id.ToString()
                };

                await _resolutionRepository.Insert(resolution);

                //Obtém a mensagem
                Message message = await MakeMessage(resolution.CallNumber);

                //Envia a mensagem
                await SendMessage(message);

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


        private static void Validate(Resolution resolution)
        {
            if (resolution.CallNumber == 0)
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.PROTOCOL_NUMBER));
            }

            if (string.IsNullOrEmpty(resolution.Comment))
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.COMMENT));
            }

            if (resolution.User != null && resolution.User.Id == "")
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.USER));
            }
        }


        /// <summary>
        /// Envia as informações ao cliente com os dados da resolução
        /// </summary>
        ///<param name="resolutionDto">Dados da Resolução</param>
        /// <returns>Flag Sucesso/erro</returns>
        public async Task<bool> SendMessage(Message message)
        {
            EmailHeaderVO emailHeaderVO = new EmailHeaderVO
            {
                CallNumber = message.CallNumber,
                CustomerName = message.Customer == null ? "" : message.Customer.Name,
                CustomerEmail = message.Customer == null ? "" : message.Customer.Email,
                Date = message.Date,
                Comment = message.Comment,
                MessageCreated = DateTime.Now
            };

            await _rabbitMQMessageSender.SendMessageAsync(emailHeaderVO, "checkoutqueueEmail");
            return true;
        }

        /// <summary>
        /// Monta a mensagem
        /// </summary>
        /// <param name="callNumber"></param>
        /// <returns></returns>
        private async Task<Message> MakeMessage(int callNumber)
        {
            CallDto callParams = new CallDto
            {
                CallNumber = callNumber.ToString(),
                StartDate = null,
                FinishDate = null,
                Customer = new ComboOption
                {
                    Id = "0"
                },
                Status = (byte)StatusEn.None
            };

            CallRequest callRequest = new CallRequest
            {
                Call = callParams
            };

            List<Call> lstCall = await _callService.GetbyParams(callRequest);

            string customerId = "0";
            Call call = new Call();
            if (lstCall != null && lstCall.Count > 0)
            {
                call = lstCall[0];
                customerId = call.Customer!.Id;
            }

            //Obtém os dados do cliente
            Customer customer = await _customerService.GetById(Convert.ToInt32(customerId));

            //Envia a mensagem para o serviço assincrono
            Message messageX = new Message
            {
                CallNumber = Convert.ToInt32(callNumber),
                Customer = new CustomerDto
                {
                    CustomerId = customer.Id.ToString(),
                    Name = customer.Name,
                    Email = customer.Email,
                },
                Date = call.FinishDate,
                Comment = "Texto da Resolução"
            };

            return messageX;
        }

        Task<bool> IResolutionService.ReadMessage()
        {
            throw new NotImplementedException();
        }
    }
}
