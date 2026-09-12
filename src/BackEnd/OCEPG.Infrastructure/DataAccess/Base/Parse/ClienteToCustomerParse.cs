using Microsoft.Data.SqlClient;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Framework.Exception;
using System.Data;

namespace OCPEG.DAL.Parse
{
    /// <summary>
    ///  Faz a Interação entre Cliente e Customer
    /// </summary>
    public partial class PegModelToBusiness
    {
        /// <summary>
        ///  Recebe uma Lista de Clientes e Gera uma Lista de Business Entity Customer
        /// </summary>
        /// <returns>Lista de Business Entity ComboOption</returns>
        ///<param name="recCustomer">Lista de Clientes</param>
        public List<Customer> ClienteToCustomer(IEnumerable<IDataRecord> recCustomer)
        {
            return (recCustomer.Select(ClienteToCustomer)).ToList();
        }

        /// <summary>
        ///  Recebe Uma Row de um RecordSet Cliente e Gera Uma Business Entity Customer
        /// </summary>
        /// <returns>Uma Business Entity Customer</returns>
        ///<param name="recCustomer">Uma Row de um RecordSet Cliente</param>
        public Customer ClienteToCustomer(IDataRecord recCustomer)
        {
            try
            {
                if (recCustomer == null)
                    return new Customer();

                return new Customer
                {
                    CustomerId = DbTools.GetValue<long>("Id", recCustomer),
                    Name = DbTools.GetValue<string>("Nome", recCustomer),
                    Email = DbTools.GetValue<string>("Email", recCustomer),
                    Telephone = DbTools.GetValue<string>("Telefone", recCustomer),
                    Age = DbTools.GetValue<short>("Idade", recCustomer),
                    EnrollmentDate = DbTools.GetValue<DateTime>("DataInscricao", recCustomer),
                    Text = DbTools.GetValue<string>("Texto", recCustomer),
                    MonthlyPayment = DbTools.GetValue<decimal>("Mensalidade", recCustomer),
                    TypeId = DbTools.GetValue<int>("TipoId", recCustomer)
                };
            }
            catch (Exception ex)
            {
                throw new OcException(string.Format("{0}.{1}: {2}", ToString(),
                                                            "ClienteToCustomer", ex.Message));
            }
        }

        /// <summary>
        ///  Recebe Uma Row de um RecordSet Cliente e Gera Uma Business Entity Customer
        /// </summary>
        /// <returns>Uma Business Entity Customer</returns>
        ///<param name="recCustomer">Uma Row de um RecordSet Cliente</param>
        public async Task<List<Customer>> ClienteDataReaderToCustomer(SqlDataReader rdr)
        {
            try
            {
                if (rdr == null)
                    return new List<Customer>();

                List<Customer> customers = new List<Customer>();

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

                    /* NOSONAR
                    /* NOSONAR  customer.CustomerId = await rdr.IsDBNullAsync(0) ? 0 : Convert.ToInt64(rdr["Id"]); */
                    /* NOSONAR  customer.Name = await rdr.IsDBNullAsync(1) ? string.Empty : rdr.GetString(1); //rdr["Nome"] */
                    /* NOSONAR  customer.Email = await rdr.IsDBNullAsync(2) ? string.Empty : rdr.GetString(2);  //rdr["Email"] */
                    /* NOSONAR  customer.Telephone = await rdr.IsDBNullAsync(3) ? string.Empty : rdr.GetString(3); //rdr["Telefone"] */
                    /* NOSONAR  customer.Age = await rdr.IsDBNullAsync(4) ? Convert.ToInt16(0) : Convert.ToInt16(rdr["Idade"]); */
                    /* NOSONAR  customer.EnrollmentDate = await rdr.IsDBNullAsync(5) ? DateTime.MinValue : Convert.ToDateTime(rdr["DataInscricao"]); */
                    /* NOSONAR  customer.Text = await rdr.IsDBNullAsync(6) ? string.Empty : rdr.GetString(6);  //rdr["Texto"] */
                    /* NOSONAR  customer.MonthlyPayment = await rdr.IsDBNullAsync(7) ? 0 : (decimal)rdr["Mensalidade"]; */
                    /* NOSONAR  customer.TypeId = await rdr.IsDBNullAsync(8) ? 0 : Convert.ToInt32(rdr["TipoId"]); */

                }

                return customers;
            }
            catch (Exception ex)
            {
                throw new OcException(string.Format("{0}.{1}: {2}", ToString(),
                                                            "ClienteDataReaderToCustomer", ex.Message));
            }
        }

    }

}
