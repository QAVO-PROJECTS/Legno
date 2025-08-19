using Microsoft.EntityFrameworkCore;

using Legno.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legno.Application.Abstracts.Repositories;

namespace Legno.Persistence.Concreters.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class, new()

    {
        private readonly LegnoDbContext _LegnoDbContext;

        public WriteRepository(LegnoDbContext LegnoDbContext)
        {
            _LegnoDbContext = LegnoDbContext;
        }

        private DbSet<T> Table { get => _LegnoDbContext.Set<T>(); }
        public async Task AddAsync(T entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task HardDeleteAsync(T entity)
        {
            await Task.Run(() => Table.Remove(entity));

        }

        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => Table.Update(entity));
            return entity;
        }

        public async Task<int> CommitAsync()
        {
            return await _LegnoDbContext.SaveChangesAsync();
        }
    }
}
