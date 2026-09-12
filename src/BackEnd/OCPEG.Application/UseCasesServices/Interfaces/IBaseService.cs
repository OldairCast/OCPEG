
namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface IBaseService<T,T2>
    {
        Task<T> GetById(int id);

        Task<List<T2>> GetAll();

        Task<int> Create(T entity);

        Task<bool> Update(T entity);

        Task<bool> Delete(int id);
    }
}
