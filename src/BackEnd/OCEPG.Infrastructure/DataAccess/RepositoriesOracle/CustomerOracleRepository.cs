using AutoMapper;
using MySqlConnector;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.DAL.Parse;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OCEPG.Infrastructure.DataAccess.RepositoriesOracle
{
    public class CustomerOracleRepository : BaseRepository, ICustomerRepository
    {
        public CustomerOracleRepository(IMapper mapper) : base(mapper)
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
                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    OracleCommand cmd = new OracleCommand("pr_sel_cliente_por_id", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = id;
                    await con.OpenAsync();
                    OracleDataReader rdr = await cmd.ExecuteReaderAsync();

                    if (rdr == null)
                        return new Customer();

                    customer.CustomerId = DbTools.GetValue<long>("Id", rdr);
                    customer.Name = DbTools.GetValue<string>("Nome", rdr);
                    customer.Email = DbTools.GetValue<string>("Email", rdr);
                    customer.Telephone = DbTools.GetValue<string>("Telefone", rdr);
                    customer.Age = DbTools.GetValue<short>("Idade", rdr);
                    customer.EnrollmentDate = DbTools.GetValue<DateTime>("DataInscricao", rdr);
                    customer.Text = DbTools.GetValue<string>("Texto", rdr);
                    customer.MonthlyPayment = DbTools.GetValue<decimal>("Mensalidade", rdr);
                    customer.TypeId = DbTools.GetValue<int>("TipoId", rdr);
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
                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    OracleCommand cmd = new OracleCommand("pr_sel_cliente", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    OracleDataReader rdr = await cmd.ExecuteReaderAsync();

                    if (rdr == null)
                        return new List<Customer>();

                    while (await rdr.ReadAsync())
                    {
                        Customer customer = new Customer
                        {
                            Id = await DbTools.GetInt32Async(rdr, 0),
                            Name = await DbTools.GetStringAsync(rdr, 1),
                            Email = await DbTools.GetStringAsync(rdr, 2),
                            Telephone = await DbTools.GetStringAsync(rdr, 3),
                            Age = await DbTools.GetInt16Async(rdr, 4),
                            EnrollmentDate = await DbTools.GetDateTimeAsync(rdr, 5),
                            Text = await DbTools.GetStringAsync(rdr, 6),
                            MonthlyPayment = await DbTools.GetDecimalAsync(rdr, 7),
                            TypeId = await DbTools.GetInt32Async(rdr, 8)
                        };

                        customers.Add(customer);
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
        /// Obtém dados do cliente por nome
        /// </summary>
        /// <param name="search">Critério de Pesquisa</param>
        /// <returns>Objeto de negocio ComboOption</returns>
        public async Task<List<ComboOption>> GetByNameCbo(string search)
        {
            try
            {
                List<ComboOption> customers = new List<ComboOption>();

                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    OracleCommand cmd = new OracleCommand("pr_sel_cliente_por_nome", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("nome", OracleDbType.Varchar2).Value = search;
                    await con.OpenAsync();
                    OracleDataReader rdr = await cmd.ExecuteReaderAsync();
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
                using (OracleConnection conn = new OracleConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    OracleCommand command = conn.CreateCommand();
                    command.CommandText = "pr_sel_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();

                    OracleDataReader resultSet = await command.ExecuteReaderAsync();

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

                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    await con.OpenAsync();
                    OracleCommand command = con.CreateCommand();

                    command.CommandText = "pr_ins_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();

                    command.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = customer.Name;
                    command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = customer.Email;
                    command.Parameters.Add("p_telefone", OracleDbType.Varchar2).Value = customer.Telephone;
                    command.Parameters.Add("p_idade", OracleDbType.Int16).Value = customer.Age;
                    command.Parameters.Add("p_dataInscricao", OracleDbType.TimeStamp).Value = customer.EnrollmentDate;
                    command.Parameters.Add("p_mensalidade", OracleDbType.Double).Value = customer.MonthlyPayment;
                    command.Parameters.Add("p_texto", OracleDbType.Clob).Value = customer.Text == null ? DBNull.Value : customer.Text;
                    command.Parameters.Add("p_tipoId", OracleDbType.Int16).Value = customer.TypeId;

                    OracleParameter oraParam = new OracleParameter()
                    {
                        ParameterName = "p_codcli",
                        OracleDbType = OracleDbType.Int32,
                        Direction = ParameterDirection.Output,
                        SourceVersion = DataRowVersion.Current,
                        IsNullable = true,
                        Value = DBNull.Value
                    };

                    command.Parameters.Add(oraParam);

                    await command.ExecuteNonQueryAsync();

                    newProdID = Convert.ToInt32(command.Parameters["p_codcli"].Value.ToString());
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
                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    await con.OpenAsync();
                    OracleCommand command = con.CreateCommand();

                    command.CommandText = "pr_upd_cliente";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.Add("p_Id", OracleDbType.Varchar2).Value = customer.CustomerId;
                    command.Parameters.Add("p_nome", OracleDbType.Varchar2).Value = customer.Name;
                    command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = customer.Email;
                    command.Parameters.Add("p_telefone", OracleDbType.Varchar2).Value = customer.Telephone;
                    command.Parameters.Add("p_idade", OracleDbType.Int16).Value = customer.Age;
                    command.Parameters.Add("p_dataInscricao", OracleDbType.TimeStamp).Value = customer.EnrollmentDate;
                    command.Parameters.Add("p_texto", OracleDbType.Clob).Value = customer.Text == null ? DBNull.Value : customer.Text;
                    command.Parameters.Add("p_mensalidade", OracleDbType.Double).Value = customer.MonthlyPayment;
                    command.Parameters.Add("p_tipoId", OracleDbType.Int32).Value = customer.TypeId;

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
                using (OracleConnection con = new OracleConnection(ConnectionString))
                {
                    OracleCommand cmd = new OracleCommand("pr_del_cliente", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    MySqlParameter paramId = new MySqlParameter();
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
