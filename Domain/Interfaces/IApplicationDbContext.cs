using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        public IDbConnection Connection { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}