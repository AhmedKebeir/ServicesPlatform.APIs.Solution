using Microsoft.EntityFrameworkCore;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Repositories.Contract;
using ServicesPlatform.Core.Specifications;
using ServicesPlatform.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Repositories
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppIdentityDbContext _dbContext;

        public GenericRepository(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).AsSplitQuery().ToListAsync();
        }

        public async Task<T?> GetWithSpec(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).AsSplitQuery().FirstOrDefaultAsync();
        }
        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }
        public async Task AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
        }
        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
        }

        private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
