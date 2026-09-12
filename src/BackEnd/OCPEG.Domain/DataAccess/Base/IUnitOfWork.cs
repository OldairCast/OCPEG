namespace OCPEG.Domain.DataAccess.Base
{
    public interface IUnitOfWork
    {
        public Task CommitSaveChanges();
    }
}
