using AutoMapper;
using Microsoft.Data.SqlClient;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.DAL.Parse;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using System.Data;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class ResolutionRepository : BaseRepository, IResolutionRepository
    {
        public ResolutionRepository(IMapper mapper) : base(mapper)
        {
        }

        /// <summary>
        /// Obtém dados da resolução por numero do protocolo
        /// </summary>
        /// <param name="callNumber">Numero do Protocolo</param>
        /// <returns>Objeto de negocio ResolutionDto</returns>
        public async Task<Resolution> GetByCallNumber(string callNumber)
        {
            var rsList = new Dictionary<string, IEnumerable<IDataRecord>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();

                    SqlCommand command = conn.CreateCommand();
                    command.CommandText = "pr_net_sel_resolucao";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@protocolo", callNumber);

                    SqlDataReader resultSet = await command.ExecuteReaderAsync();

                    if (resultSet.HasRows)
                    {
                        rsList.Add("resolution", resultSet.Cast<IDataRecord>().ToList());
                        await conn.CloseAsync();
                    }
                    else
                    {
                        await conn.CloseAsync();
                        return new Resolution();
                    }
                }

                //Instancia classe geral de parsing para objetos de negocio
                PegModelToBusiness parse = new PegModelToBusiness();

                IDataRecord? recParameter = rsList["resolution"] != null ? rsList["resolution"].FirstOrDefault() : null; 

                if (recParameter == null)
                    return await Task.Run(() => new Resolution());

                Resolution patch = parse.ResolucaoToResolution(recParameter);

                return await Task.Run(() => patch);

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Realiza a resolução de um atendimento
        /// </summary>
        ///<param name="resolutionDto">Dados da Resolução</param>
        /// <returns>Id do protocolo criado</returns>
        public Task<bool> Insert(Resolution resolutionDto)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    SqlCommand command = con.CreateCommand();

                    command.CommandText = "pr_ins_resolucao";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@protocolo", resolutionDto.CallNumber);
                    command.Parameters.AddWithValue("@dataSolucao", resolutionDto.Date);
                    command.Parameters.AddWithValue("@comentario", resolutionDto.Comment);
                    command.Parameters.AddWithValue("@usuario", resolutionDto.User!.Id);

                    command.ExecuteNonQuery();
                    con.Close();
                }

                return Task.Run(() => true);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }
    }
}

