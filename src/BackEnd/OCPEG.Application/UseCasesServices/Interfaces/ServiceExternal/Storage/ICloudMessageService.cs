namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage
{
    public interface ICloudMessageService
    {
        Task<bool> DeleteUser(int userId);

        Task<bool> DeleteUserNoAzure(int userId);
    }
}
