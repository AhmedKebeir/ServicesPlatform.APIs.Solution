using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Repositories.Contract
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        Task<T?> GetWithSpec(ISpecifications<T> spec);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<int> GetCountAsync(ISpecifications<T> spec);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
