using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Framework.Exception;
using System.Data;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.DAL.Parse
{
    /// <summary>
    ///  Faz a Interação entre Atendimento e Call
    /// </summary>
    public partial class PegModelToBusiness
    {
        /// <summary>
        ///  Recebe uma Lista de Atendimento e Gera uma Lista de Business Entity Call
        /// </summary>
        /// <returns>Lista de Business Entity Call</returns>
        ///<param name="recCall">Lista de Atendimento</param>
        public List<Call> AtendimentoToCall(IEnumerable<IDataRecord> recCall)
        {
            return (recCall.Select(AtendimentoToCall)).ToList();
        }

        /// <summary>
        ///  Recebe Uma Row de um RecordSet Atendimento e Gera Uma Business Entity Call
        /// </summary>
        /// <returns>Uma Business Entity Call</returns>
        ///<param name="recCall">Uma Row de um RecordSet Atendimento</param>
        public Call AtendimentoToCall(IDataRecord recCall)
        {
            try
            {
                if (recCall == null)
                    return new Call();

                return new Call
                {
                    CallNumber = DbTools.GetValue<int>("Protocolo", recCall),
                    StartDate = DbTools.GetValue<DateTime>("DataInicio", recCall),
                    FinishDate = DbTools.GetValue<DateTime>("DataConclusao", recCall),
                    Comment = DbTools.GetValue<string>("Comentario", recCall),
                    Product = new Product
                    {
                        Id = DbTools.GetValue<int>("IdAssunto", recCall),
                        Name = DbTools.GetValue<string>("DescricaoAssunto", recCall)
                    },
                    Priority = (PriorityEn)DbTools.GetValue<byte>("Prioridade", recCall),
                    Status = (StatusEn)DbTools.GetValue<byte>("StatusAtend", recCall),
                    Customer = new ComboOption
                    {
                        Id = DbTools.GetValue<string>("IdCliente", recCall) ?? "",
                        Name = DbTools.GetValue<string>("NomeCliente", recCall)
                    },
                };
            }
            catch (Exception ex)
            {
                throw new OcException(string.Format("{0}.{1}: {2}", ToString(),
                                                            "AtendimentoToCall", ex.Message));
            }
        }
    }
}
