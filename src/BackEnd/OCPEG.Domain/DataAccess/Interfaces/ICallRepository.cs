using OCPEG.Domain.BusinessObject;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface ICallRepository
    {
        Task<List<Call>> GetbyParams(Call callParams);
        
        Task<int> Create(Call call);

        Task<bool> Update(Call call);
    }
}
