﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using TP.Upgrade.Application.Common.Contracts.IDataManagers;
using TP.Upgrade.Domain.Enums;

namespace TP.Upgrade.Infrastructure.DataManagers
{
    public class Repository<TEntity> : IRepository<TEntity>
         where TEntity : class
    {

        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual IQueryable<TEntity> GetWithCondition(Expression<Func<TEntity, bool>> expression)
        {
            return _dbContext.Set<TEntity>().Where(expression);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }
        public IQueryable<TEntity> GetReadOnlyList()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }
        public async Task<TEntity> Get<T>(T id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<int> Add(TEntity entity, bool isSaveChanges = true)
        {
            SetCreateAnalysisValue(entity);
            await _dbContext.Set<TEntity>().AddAsync(entity);
            if (isSaveChanges)
            {
                return await Save();
            }
            return 0;
        }

        public async Task Change(TEntity entity, bool isSaveChanges = true)
        {
            SetUpdateAnalysisValue(entity, false);
            _dbContext.Set<TEntity>().Update(entity);
            if (isSaveChanges)
            {
                await Save();
            }
        }

        public async Task Delete<T>(T id, bool isSaveChanges = true)
        {

            var entity = await Get(id);


            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);
                if (isSaveChanges)
                {
                    await Save();
                }
            }

        }
        public async Task DeleteRange<T>(List<T> ids, bool isSaveChanges = true)
        {
            foreach (var item in ids)
            {
                await Delete(item, isSaveChanges);
            }
        }
        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }



        public IQueryable<TEntity> GetNotDeleted()
        {
            object deleted = true;
            return GetAll().Where(x => typeof(TEntity).GetProperty("IsDeleted").GetValue(x) != deleted);
        }

        //public async Task SoftDelete(TEntity entity)
        //{
        //    entity.IsDeleted = true;
        //    entity.DeletedDate = DateTime.UtcNow;
        //    await Save();
        //}
        public async Task SoftDelete(object entity)
        {
            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
            {
                isDeletedProperty.SetValue(entity, true);
                var deletedDateProperty = entity.GetType().GetProperty("DeletedDate");
                if (deletedDateProperty != null && deletedDateProperty.PropertyType == typeof(DateTime?))
                {
                    deletedDateProperty.SetValue(entity, DateTime.UtcNow);
                    await Save();
                }
            }
        }
        public async Task RemoveRangeAsync(List<TEntity> entities, bool isSaveChanges = true)
        {
            _dbContext.RemoveRange(entities);
            if (isSaveChanges)
                await Save();


        }

        public async Task AddRangeAsync(List<TEntity> entities, bool isSaveChanges = true)
        {
            foreach (var item in entities)
                SetCreateAnalysisValue(item);
            await _dbContext.AddRangeAsync(entities);
            if (isSaveChanges)
                await Save();
        }

        public async Task SeederAddRangeAsync(List<TEntity> entities, bool isSaveChanges = true)
        {
            await _dbContext.AddRangeAsync(entities);
            if (isSaveChanges)
                await Save();
        }

        public async Task ChangeRange(List<TEntity> entities, bool isSaveChanges = true)
        {
            foreach (var item in entities)
                SetUpdateAnalysisValue(item, false);
            _dbContext.Set<TEntity>().UpdateRange(entities);
            if (isSaveChanges)
                await Save();
        }

        public async Task SeederChangeRange(List<TEntity> entities, bool isSaveChanges = true)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
            if (isSaveChanges)
                await Save();
        }

        private void SetCreateAnalysisValue(TEntity entity)
        {
            var entityProperties = entity.GetType().GetProperties();
            var createdDateProperty = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "createddate");
            var isActiveProperty = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "isactive");
            if (createdDateProperty != null)
                createdDateProperty.SetValue(entity, DateTime.UtcNow);
            if (isActiveProperty != null)
                isActiveProperty.SetValue(entity, true);
        }

        private void SetUpdateAnalysisValue(TEntity entity, bool isSoftDelete)
        {
            var entityProperties = entity.GetType().GetProperties();
            var property = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "modifieddate");
            if (property != null)
                property.SetValue(entity, DateTime.UtcNow);

            if (isSoftDelete)
            {
                property = entityProperties.FirstOrDefault(x => x.Name.ToLower() == "isdeleted");
                if (property != null)
                    property.SetValue(entity, true);
            }
        }

        public IQueryable<T> ExecuteStoredProcedure<T>(string procedure, params object[] parameters)
        {
            return _dbContext.Database.SqlQueryRaw<T>($"EXEC {procedure}", parameters);
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            return transaction.GetDbTransaction();
        }
    }
}
