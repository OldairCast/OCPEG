using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using System.Globalization;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CallService: ICallService
    {
        private readonly ICallRepository _callRepository;
        private readonly IAccountService _accountService;

        public CallService(ICallRepository callRepository,IAccountService accountService)
        {
            _callRepository = callRepository;
            _accountService = accountService;
        }

        /// <summary>
        /// Retorna lista de atendimentos
        /// </summary>
        /// <returns>Lista de Objeto de negocio CallDto</returns>
        public async Task<List<Call>> GetAll()
        {
            try
            {
                Call callParams = new Call
                {
                    CallNumber = 0,
                    StartDate = null,
                    FinishDate = null,
                    Customer = new ComboOption
                    {
                        Id = "0"
                    },
                    Status = StatusEn.None
                };

                List<Call> callLst = await _callRepository.GetbyParams(callParams);

                return callLst;
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
        /// Retorna lista de atendimentos conforme parãmetros
        /// </summary>
        /// <param name="request">Lista de Parãmetros de pesquisa</param>
        /// <returns>Lista de Objeto de negocio CallDto</returns>
        public async Task<List<Call>> GetbyParams(CallRequest request)
        {
            try
            {
                DateTime dateIni = DateTime.MinValue;
                DateTime dateFim = DateTime.MinValue;
                int callNumber = 0;
                StatusEn statusEnum = StatusEn.None;

                ComboOption Customer = new ComboOption();

                //Obtém as datas do período
                if (request.Call != null)
                {
                    if (request.Call.StartDate != null && request.Call.StartDate != DateTime.MinValue)
                    {
                        string startDate = request.Call.StartDate.ToString() ?? dateIni.ToString();
                        dateIni = MakePeriod(startDate);
                    }

                    if (request.Call.FinishDate != null && request.Call.FinishDate != DateTime.MinValue)
                    {
                        string finishDate = request.Call.FinishDate.ToString() ?? dateFim.ToString();
                        dateFim = MakePeriod(finishDate);
                    }

                    string customerId = request.Call.Customer!.Id.ToString();
                    Customer.Id = customerId;

                    callNumber = Convert.ToInt32(request.Call.CallNumber);
                    statusEnum = request.Call.Status;
                }

                Call call = new Call
                {
                    CallNumber = callNumber,
                    StartDate = dateIni,
                    FinishDate = dateFim,
                    Customer = Customer,
                    Status = statusEnum
                };

                List<Call> callLst = await _callRepository.GetbyParams(call);

                return callLst;
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
        /// Insere um novo atendimento
        /// </summary>
        /// <param name="call"></param>
        /// <returns>Id do protocolo criado</returns>
        public async Task<CallResponse> Create(Call call)
        {
            try
            {
                Validate(call);

                var objUser = await _accountService.GetUserLogged();
                var user = objUser;

                call.StartDate = DateTime.Now;
                call.StartDate = DateTime.Now.AddDays(3);
                call.Status = StatusEn.Aberto;
                call.UserId = user.Id;


                int callNumber = await _callRepository.Create(call);

                CallResponse callNew = new CallResponse
                {
                    CallNumber = callNumber,
                    PreviousDate = call.StartDate
                };

                return callNew;
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
        /// Atualiza um Atendimento aberto
        /// </summary>
        /// <param name="call">Dados do Atendimento</param> 
        public async Task<bool> Update(Call call)
        {
            try
            {
                await _callRepository.Update(call);

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
        /// 
        /// </summary>
        /// <param name="call"></param>
        /// <exception cref="BusinessLogicCustomException"></exception>
        private static void Validate(Call call)
        {
            string msg_format = "{0} {1}";
            
            if (string.IsNullOrEmpty(call.Comment))
            {
                throw new BusinessLogicCustomException(string.Format(msg_format, OcPegResource.ENTER_THE, OcPegResource.COMMENT));
            }

            if (call.Customer == null || call.Customer.Id == "0")
            {
                throw new BusinessLogicCustomException(string.Format(msg_format, OcPegResource.ENTER_THE, OcPegResource.CUSTOMER));
            }

            if (call.Product != null && call.Product.Id == 0)
            {
                throw new BusinessLogicCustomException(string.Format(msg_format, OcPegResource.ENTER_THE, OcPegResource.SUBJECT));
            }

            if (Convert.ToByte(call.Priority) == 0)
            {
                throw new BusinessLogicCustomException(string.Format(msg_format, OcPegResource.ENTER_THE, OcPegResource.PRIORITY));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime MakePeriod(string date)
        {
            string day;
            string month;
            string year;

            day = date.Substring(0, 2);
            month = date.Substring(3, 2);
            year = date.Substring(6, 4);

            string culture = CultureInfo.CurrentCulture.Name;

            DateTime dateP = culture == "en-US" ?
                Convert.ToDateTime(month + "/" + day + "/" + year)
                : Convert.ToDateTime(day + "/" + month + "/" + year);

            return dateP;
        }
    }
}
