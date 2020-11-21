using Messanger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Messanger.Contexts
{
    public interface IDbRepository
    {
        T GetFirst<T>(Expression<Func<T, bool>> selector) where T : BaseEntity;
        List<T> Get<T>(Expression<Func<T, bool>> selector) where T : BaseEntity;
        List<T> Get<T>() where T : BaseEntity;
        Task<Guid> Add<T>(T newEntity) where T : BaseEntity;
        Task AddRange<T>(IEnumerable<T> newEntities) where T : BaseEntity;
        //Task Delete<T>(Guid entity) where T : BaseEntity;
        Task Remove<T>(T entity) where T : BaseEntity;
        Task RemoveRange<T>(IEnumerable<T> entities) where T : BaseEntity;
        Task Update<T>(T entity) where T : BaseEntity;
        Task UpdateRange<T>(IEnumerable<T> entities) where T : BaseEntity;
        Task<int> SaveChangesAsync();
    }

    public class DbRepository : IDbRepository
    {
        public readonly MessangerDbContext context;

        public DbRepository(MessangerDbContext context)
        {
            this.context = context;
        }

        public List<T> Get<T>() where T: BaseEntity
        {
            return this.context.Set<T>().ToList();
        }

        public List<T> Get<T>(Expression<Func<T, bool>> selector) where T : BaseEntity
        {
            return this.context.Set<T>().Where(selector).ToList();
        }

        public T GetFirst<T>(Expression<Func<T, bool>> selector) where T : BaseEntity
        {
            return this.context.Set<T>().FirstOrDefault(selector);
        }

        public async Task<Guid> Add<T>(T newEntity) where T : BaseEntity
        {
            var entity = await this.context.Set<T>().AddAsync(newEntity);
            return entity.Entity.Id;
        }

        public async Task AddRange<T>(IEnumerable<T> newEntities) where T : BaseEntity
        {
            await this.context.Set<T>().AddRangeAsync(newEntities);
        }

        public async Task Remove<T>(T entity) where T : BaseEntity
        {
            await Task.Run(() => this.context.Set<T>().Remove(entity));
        }

        public async Task RemoveRange<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            await Task.Run(() => this.context.Set<T>().RemoveRange(entities));
        }

        public async Task Update<T>(T entity) where T : BaseEntity
        {
            await Task.Run(() => this.context.Set<T>().Update(entity));
        }

        public async Task UpdateRange<T>(IEnumerable<T> entities) where T : BaseEntity
        {
            await Task.Run(() => this.context.Set<T>().UpdateRange(entities));
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync();
        }
    }
}
