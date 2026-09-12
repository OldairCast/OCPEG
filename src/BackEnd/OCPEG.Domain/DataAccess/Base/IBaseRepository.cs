namespace OCPEG.Domain.DataAccess.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);

        Task<List<T>> GetAll();

        Task<int> Create(T entity);

        Task<bool> Update(T entity);

        Task<bool> Delete(int id);

    }
}
