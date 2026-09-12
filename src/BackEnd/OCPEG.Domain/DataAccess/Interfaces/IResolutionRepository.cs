using OCPEG.Domain.BusinessObject;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface IResolutionRepository
    {
        Task<Resolution> GetByCallNumber(string callNumber);

        Task<bool> Insert(Resolution resolutionDto);
    }
}
