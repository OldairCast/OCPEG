using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Base;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class CustomerEfRepository : BaseRepository, ICustomerEfRepository
    {
        private readonly AppDbContext _dbContext;

        public CustomerEfRepository(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Obtém dados do cliente por id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Objeto de negocio Customer</returns>
        public async Task<Customer> GetById(int id)
        {
            try
            {
                Customer customer = new Customer();

                Cliente? cliente = await _dbContext.Cliente.FindAsync(id);
                if (cliente != null)
                {
                    customer.Id = cliente.Id;
                    customer.Name = cliente.Nome ?? string.Empty;
                    customer.Email = cliente.Email ?? string.Empty;
                    customer.Telephone = cliente.Telefone ?? string.Empty;
                    customer.Age = cliente.Idade ?? 0;
                    customer.EnrollmentDate = cliente.DataInscricao;
                    customer.Text = cliente.Texto ?? string.Empty;
                    customer.MonthlyPayment = cliente.Mensalidade ?? 0;
                    customer.TypeId = cliente.TipoId;
                }

                return customer;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Retorna lista de clientes
        /// </summary>
        /// <returns>Lista de Objeto de negocio Customer</returns>
        public async Task<List<Customer>> GetAll()
        {
            try
            {
                List<Customer> customers = new List<Customer>();

                List<Cliente> clientes = await _dbContext.Cliente.ToListAsync();

                foreach (var item in clientes)
                {
                    Customer customer = new Customer
                    {
                        Id = item.Id,
                        Name = item.Nome ?? string.Empty,
                        Email = item.Email ?? string.Empty,
                        Telephone = item.Telefone ?? string.Empty,
                        Age = item.Idade ?? 0,
                        EnrollmentDate = item.DataInscricao,
                        Text = item.Texto ?? string.Empty,
                        MonthlyPayment = item.Mensalidade ?? 0,
                        TypeId = item.TipoId
                    };

                    customers.Add(customer);
                }

                return customers;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Insere um novo cliente
        /// </summary>
        /// <returns>Id do cliente criado</returns>
        public async Task<long> Create(Customer customer)
        {
            try
            {
                Cliente cliente = new Cliente
                {
                    Nome = customer.Name,
                    Email = customer.Email,
                    Telefone = customer.Telephone,
                    Idade = customer.Age ?? 0,
                    DataInscricao = customer.EnrollmentDate,
                    Texto = customer.Text,
                    Mensalidade = customer.MonthlyPayment ?? 0,
                    TipoId = customer.TypeId
                };

                await _dbContext.Cliente.AddAsync(cliente);

                Customer customerRet = _mapper.Map<Customer>(cliente);
                return customerRet.Id;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza um cliente
        /// </summary>
        /// <param name="customer">Dados do cliente</param>
        public async Task<bool> Update(Customer customer)
        {
            try
            {
                Cliente? cliente = await _dbContext.Cliente.FindAsync(customer.Id);
                if (cliente != null)
                {
                    cliente.Nome = customer.Name;
                    cliente.Email = customer.Email;
                    cliente.Telefone = customer.Telephone;
                    cliente.Idade = customer.Age != null ? customer.Age : 0;
                    cliente.DataInscricao = customer.EnrollmentDate != null ? (DateTime)customer.EnrollmentDate : null;
                    cliente.Texto = customer.Text;
                    cliente.Mensalidade = customer.MonthlyPayment != null ? (decimal)customer.MonthlyPayment : 0;

                    _dbContext.Cliente.Update(cliente);
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
        /// Exclui um cliente
        /// </summary>
        /// <param name="id">Id do cliente</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                Cliente? customer = await _dbContext.Cliente.FindAsync(id);

                if (customer != null)
                {
                    _dbContext.Cliente.Remove(customer);
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

        Task<int> IBaseRepository<Customer>.Create(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
