using AutoMapper;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Validator;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Base;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using static OCPEG.Framework.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém dados do Usuário por email
        /// </summary>
        /// <param name="id">email do Usuário</param>
        /// <returns>Objeto de negocio User</returns>
        public async Task<User> GetByEmail(string email)
        {
            try
            {
                User user = await _userRepository.GetByEmail(email);
                return user;
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
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetById(int id)
        {
            try
            {
                User user = await _userRepository.GetById(id);
                return user;

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
        /// Retorna lista de Usuário
        /// </summary>
        /// <returns>Lista de Objeto de negocio Usuario</returns>
        public async Task<List<UserDto>> GetAll()
        {
            try
            {
                List<User> users = await _userRepository.GetAll();

                List<UserDto> usersDto = new List<UserDto>();

                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        UserDto userDto = _mapper.Map<UserDto>(item);
                        usersDto.Add(userDto);
                    }
                }

                return usersDto;

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
        /// Insere um novo Usuário
        /// </summary>
        /// <returns>Objeto Usuário criado</returns>
        public async Task<int> Create(User entity)
        {
            try
            {
                await Validate(entity, true);

                string email = entity.Email != null ? entity.Email.Trim().ToLower() : string.Empty;
                var emailExist = await _userRepository.ExistActiveUserWithEmail(email);
                if (emailExist)
                    throw new BusinessLogicCustomException(OcPegResource.EMAIL_INVALID);

                var result = await _userRepository.Create(entity);

                return result;

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
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="customer">Dados do Tipo de Cliente</param> 
        public async Task<bool> Update(User entity)
        {
            try
            {
                await Validate(entity, false);

                await _userRepository.Update(entity);

                //Finaliza a transação
                await _unitOfWork.CommitSaveChanges();

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
        /// Exclui um Usuário
        /// </summary>
        /// <param name="id">Id do Usuário</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                await _userRepository.Delete(id);

                //Finaliza a transação
                await _unitOfWork.CommitSaveChanges();

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

        private static async Task Validate(User request, bool pwd)
        {
            var validator = new UserValidator(pwd);

            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                var errorMessages = string.Join("<br>", result.Errors);

                /* NOSONAR
                /* NOSONAR var errorMessages = "";
                /* NOSONAR foreach (var error in result.Errors)
                /* NOSONAR {
                /* NOSONAR    errorMessages = errorMessages + error + "<br>";
                /* NOSONAR }
                */

                throw new BusinessLogicCustomException(errorMessages);
            }
        }

    }
}
