using AutoMapper;
using Microsoft.Data.SqlClient;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.DAL.Parse;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using System.Data;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class CallRepository : BaseRepository, ICallRepository
    {
        private readonly AppDbContext _dbContext;

        public CallRepository(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retorna lista de atendimentos conforme parãmetros
        /// </summary>
        /// <param name="callParams">Lista de Parãmetros de pesquisa</param>
        /// <returns>Lista de Objeto de negocio CallDto</returns>
        public async Task<List<Call>> GetbyParams(Call callParams)
        {
            try
            {
                Dictionary<string, IEnumerable<IDataRecord>> rsList = new Dictionary<string, IEnumerable<IDataRecord>>();

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = "pr_net_sel_atendimento";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@protocolo", callParams.CallNumber);

                    if (callParams.StartDate == null || callParams.StartDate == DateTime.MinValue)
                    {
                        command.Parameters.AddWithValue("@datainicio", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@datainicio", callParams.StartDate);
                    }

                    if (callParams.StartDate == null || callParams.StartDate == DateTime.MinValue)
                    {
                        command.Parameters.AddWithValue("@dataconclusao", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@dataconclusao", callParams.FinishDate);
                    }

                    command.Parameters.AddWithValue("@idcliente", (callParams.Customer != null && callParams.Customer.Id != "") ? Convert.ToInt32(callParams.Customer.Id) : 0);
                    
                    command.Parameters.AddWithValue("@statusAtend", Convert.ToByte(callParams.Status));

                    SqlDataReader resultSet = await command.ExecuteReaderAsync();

                    rsList.Add("atendimentos", resultSet.Cast<IDataRecord>().ToList());

                    await conn.CloseAsync();
                }

                PegModelToBusiness parse = new PegModelToBusiness();

                if (!rsList["atendimentos"].Any())
                    return new List<Call>();

                var customers = parse.AtendimentoToCall(rsList["atendimentos"]);
                return await Task.Run(() => customers);
            }
            catch (Exception ex) 
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Insere um novo atendimento
        /// </summary>
        /// <returns>Id do protocolo criado</returns>
        public async Task<int> Create(Call call)
        {
            try
            {
                int newCall;

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    await con.OpenAsync();
                    SqlCommand command = con.CreateCommand();

                    command.CommandText = "pr_ins_atendimento";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@datainicio", call.StartDate);
                    command.Parameters.AddWithValue("@comentario", call.Comment);
                    command.Parameters.AddWithValue("@idcliente", call.Customer != null ? call.Customer.Id : 0);
                    command.Parameters.AddWithValue("@idassunto", call.Product != null ? call.Product.Id : 0);
                    command.Parameters.AddWithValue("@prioridade", Convert.ToByte(call.Priority));
                    command.Parameters.AddWithValue("@statusAtend", Convert.ToByte(call.Status));
                    command.Parameters.AddWithValue("@protocolo", DBNull.Value);
                    command.Parameters.AddWithValue("@usuarioid", call.UserId);

                    var tNewCall = await command.ExecuteScalarAsync();
                    newCall = tNewCall != null ? (int)tNewCall : 0;

                    await con.CloseAsync();
                }

                return await Task.Run(() => newCall);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza um Atendimento Aberto
        /// </summary>
        /// <param name="call">Atendimento aberto</param>        
        public async Task<bool> Update(Call call)
        {
            try
            {
                Atendimento? atendimento = await _dbContext.Atendimento.FindAsync(call.CallNumber);

                if (atendimento != null)
                {
                    atendimento.Comentario = call.Comment;
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

    }
}
