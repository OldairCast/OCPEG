using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class CustomerTypeRepository : BaseRepository, ICustomerTypeRepository
    {
        private readonly AppDbContext _dbContext;

        public string? Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public CustomerTypeRepository(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Obtém dados do Tipo de Cliente por id
        /// </summary>
        /// <param name="id">Id do tipo de cliente</param>
        /// <returns>Objeto de negocio CustomerType</returns>
        public async Task<CustomerType> GetById(int id)
        {
            try
            {
                CustomerType type = new CustomerType();

                TipoCliente? tipo = await _dbContext.TipoCliente.FindAsync(id);
                if (tipo != null)
                {
                    type = _mapper.Map<CustomerType>(tipo);
                }

                return type;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Retorna lista de Tipo de Cliente
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerType</returns>
        public async Task<List<CustomerType>> GetAll()
        {
            try
            {
                List<CustomerType> typeDtolst = new List<CustomerType>();

                List<TipoCliente> tipolst = await _dbContext.TipoCliente.ToListAsync();

                foreach (var item in tipolst)
                {
                    CustomerType type = new CustomerType
                    {
                        Id = item.Id,
                        Name = item.Nome != null ? item.Nome : string.Empty,
                    };

                    typeDtolst.Add(type);
                }

                return typeDtolst;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Insere um novo Tipo de Cliente
        /// </summary>
        /// <returns>Objeto do Tipo de Cliente criado</returns>
        public async Task<int> Create(CustomerType customerType)
        {
            try
            {
                TipoCliente tipo = new TipoCliente
                {
                    Nome = customerType.Name,
                };

                await _dbContext.TipoCliente.AddAsync(tipo);
                await _dbContext.SaveChangesAsync();

                CustomerType customerTypeRet = _mapper.Map<CustomerType>(tipo);
                return customerTypeRet.Id;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="customer">Dados do Tipo de Cliente</param> 
        public async Task<bool> Update(CustomerType customerType)
        {
            try
            {
                TipoCliente? tipo = await _dbContext.TipoCliente.FindAsync(customerType.Id);
                if (tipo != null)
                {
                    tipo.Nome = customerType.Name;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Exclui um Tipo de Cliente
        /// </summary>
        /// <param name="id">Id do Tipo de Cliente</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                TipoCliente? tipo = await _dbContext.TipoCliente.FindAsync(id);

                if (tipo != null)
                {
                    _dbContext.TipoCliente.Remove(tipo);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }
    }
}
