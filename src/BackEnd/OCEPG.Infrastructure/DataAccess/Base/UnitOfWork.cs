using OCPEG.Domain.DataAccess.Base;

namespace OCEPG.Infrastructure.DataAccess.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public async Task CommitSaveChanges()
        {
            await _context.SaveChangesAsync();
        }

    }
}
