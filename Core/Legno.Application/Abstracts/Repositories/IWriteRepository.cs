using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Repositories
{
    public interface IWriteRepository<T> where T : class, new()
    {
        Task AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task HardDeleteAsync(T entity);
        Task<int> CommitAsync();
    }
}
