using AutoMapper;
using Microsoft.Data.SqlClient;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.DAL.Parse;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using System.Data;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {
        public CustomerRepository(IMapper mapper) : base(mapper)
        {
        }

        /// <summary>
        /// Obtém dados do cliente por id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Objeto de negocio CustomerDto</returns>
        public async Task<Customer> GetById(int id)
        {
            Customer customer = new Customer();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("pr_sel_cliente_por_id", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();


                    PegModelToBusiness parse = new PegModelToBusiness();
                    var customers = await parse.ClienteDataReaderToCustomer(rdr);
                    customer = customers[0];
                }

                return await Task.Run(() => customer);
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
        /// <returns>Lista de Objeto de negocio CustomerDto</returns>
        public async Task<List<Customer>> GetAll()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("pr_sel_cliente", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    PegModelToBusiness parse = new PegModelToBusiness();
                    customers = await parse.ClienteDataReaderToCustomer(rdr);

                }

                return await Task.Run(() => customers);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Obtém dados do cliente por nome
        /// </summary>
        /// <param name="search">Critério de Pesquisa</param>
        /// <returns>Objeto de negocio ComboOption</returns>
        public async Task<List<ComboOption>> GetByNameCbo(string search)
        {
            try
            {
                List<ComboOption> customers = new List<ComboOption>();

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("pr_sel_cliente_por_nome", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nome", search);
                    await con.OpenAsync();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (await rdr.ReadAsync())
                    {
                        customers.Add(new ComboOption()
                        {
                            Id = await rdr.IsDBNullAsync(0) ? "0" : rdr.GetInt32(0).ToString(),  //rdr["Id"]
                            Name = await rdr.IsDBNullAsync(1) ? string.Empty : rdr.GetString(1) //rdr["Nome"]
                        });
                    }
                }

                return await Task.Run(() => customers);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }


        /// <summary>
        /// Retorna lista de clientes utilizando parse na conversão do datareader
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerDto</returns>
        public async Task<List<Customer>> GetParse()
        {
            Dictionary<string, IEnumerable<IDataRecord>> rsList = new Dictionary<string, IEnumerable<IDataRecord>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = "pr_sel_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();

                    SqlDataReader resultSet = await command.ExecuteReaderAsync();

                    rsList.Add("clientes", resultSet.Cast<IDataRecord>().ToList());

                    await conn.CloseAsync();
                }

                PegModelToBusiness parse = new PegModelToBusiness();

                if (!rsList["clientes"].Any())
                    return new List<Customer>();

                IEnumerable<Customer> customers = parse.ClienteToCustomer(rsList["clientes"]);

                return await Task.Run(() => customers.ToList());

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
        public async Task<int> Create(Customer customer)
        {
            try
            {
                int newProdID;

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    await con.OpenAsync();
                    SqlCommand command = con.CreateCommand();

                    command.CommandText = "pr_ins_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@nome", customer.Name);
                    command.Parameters.AddWithValue("@email", customer.Email);
                    command.Parameters.AddWithValue("@telefone", customer.Telephone);
                    command.Parameters.AddWithValue("@idade", customer.Age);
                    command.Parameters.AddWithValue("@dataInscricao", customer.EnrollmentDate);
                    command.Parameters.AddWithValue("@mensalidade", customer.MonthlyPayment);
                    command.Parameters.AddWithValue("@Texto", customer.Text == null ? DBNull.Value : customer.Text);
                    command.Parameters.AddWithValue("@tipoId", customer.TypeId);
                    command.Parameters.AddWithValue("@codcli", DBNull.Value);

                    var prod = await command.ExecuteScalarAsync();
                    newProdID = prod != null ? (int)prod : 0;
                    await con.CloseAsync();
                }

                return await Task.Run(() => newProdID);
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
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    await con.OpenAsync();
                    SqlCommand command = con.CreateCommand();

                    command.CommandText = "pr_upd_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Id", customer.CustomerId);
                    command.Parameters.AddWithValue("@nome", customer.Name);
                    command.Parameters.AddWithValue("@email", customer.Email);
                    command.Parameters.AddWithValue("@telefone", customer.Telephone);
                    command.Parameters.AddWithValue("@idade", customer.Age);
                    command.Parameters.AddWithValue("@dataInscricao", customer.EnrollmentDate);
                    command.Parameters.AddWithValue("@mensalidade", customer.MonthlyPayment);
                    command.Parameters.AddWithValue("@Texto", customer.Text == null ? DBNull.Value : customer.Text);
                    command.Parameters.AddWithValue("@tipoId", customer.TypeId);

                    await command.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                return await Task.Run(() => true);
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
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("pr_del_cliente", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramId = new SqlParameter();
                    paramId.ParameterName = "@id";
                    paramId.Value = id;
                    cmd.Parameters.Add(paramId);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    await con.CloseAsync();
                }

                return await Task.Run(() => true);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

    }
}
