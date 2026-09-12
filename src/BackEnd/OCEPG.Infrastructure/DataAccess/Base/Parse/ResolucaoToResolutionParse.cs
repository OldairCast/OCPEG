using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Framework.Exception;
using System.Data;


namespace OCPEG.DAL.Parse
{

    /// <summary>
    ///  Faz a Interação entre Resolucao e Resolution
    /// </summary>
    public partial class PegModelToBusiness
    {
        /// <summary>
        ///  Recebe uma Lista de Resolucao e Gera uma Lista de Business Entity Resolution
        /// </summary>
        /// <returns>Lista de Business Entity Resolution</returns>
        ///<param name="recResolution">Lista de Resolucao</param>
        public List<Resolution> ResolucaoToResolution(IEnumerable<IDataRecord> recResolution)
        {
            return (recResolution.Select(ResolucaoToResolution)).ToList();
        }

        /// <summary>
        ///  Recebe Uma Row de um RecordSet Resolucao e Gera Uma Business Entity Resolution
        /// </summary>
        /// <returns>Uma Business Entity Resolution</returns>
        ///<param name="recResolution">Uma Row de um RecordSet Resolucao</param>
        public Resolution ResolucaoToResolution(IDataRecord recResolution)
        {
            try
            {
                if (recResolution == null)
                    return new Resolution();

                return new Resolution
                {
                    CallNumber = DbTools.GetValue<int>("Protocolo", recResolution),
                    Date = DbTools.GetValue<DateTime>("DataSolucao", recResolution),
                    Comment = DbTools.GetValue<string>("Comentario", recResolution),
                    User = new ComboOption {
                        Id = DbTools.GetValue<string>("UsuarioId", recResolution) ?? "" 
                    }
                };
            }
            catch (Exception ex)
            {
                throw new OcException(string.Format("{0}.{1}: {2}", ToString(),
                                                            "ResolucaoToResolution", ex.Message));
            }
        }
    }
}
