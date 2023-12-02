using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Persistence.Repositories;

public class EFRepositoryBase<TEntity, TEntityIdType, TContext>(TContext context)
    : IAsyncRepository<TEntity, TEntityIdType>
    , IRepository<TEntity, TEntityIdType>
    where TEntity : Entity<TEntityIdType>
    where TContext : DbContext
{
    protected readonly TContext Context = context;

    #region Async
    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Add</b> 
    ///     an <paramref name="entity"/> to connected database while also assigning the create date of the record
    /// </summary>
    /// 
    /// <param name="entity">
    ///     Represents the entity with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous Add operation. 
    ///     The task result contains the entity on which the operation was performed.
    /// </returns>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        //will be removed when Audit operation is implemented
        entity.CreatedDate = DateTime.Now;
        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Add</b> a list of 
    ///     <paramref name="entities"/> to connected database while also assigning the create date of the record
    /// </summary>
    /// 
    /// <param name="entities">
    ///     Represents the list of entities with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    ///     
    /// <returns>
    ///     A task that represents the asynchronous AddRange operation. 
    ///     The task result contains the list of entities on which the operation was performed.
    /// </returns>
    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
            entity.CreatedDate = DateTime.UtcNow;
        //will be switched to AddRange variant when Audit operation is implemented
        await Context.AddAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    ///     <b>Asynchronously</b> determines whether a sequence contains any elements. 
    ///     Within the method, it can accept parameters to modify the query.
    /// </summary>
    /// 
    /// <param name="filter">
    ///     Represents the linq query which will modify the default query
    /// </param>
    /// 
    /// <param name="withDeleted">
    ///     Represents the boolean flag which will determine whether to fetch soft deleted records
    /// </param>
    /// 
    /// <param name="enableTracking">
    ///     Represents the boolean flag which will determine whether to let EF Core to 
    ///     track the entity of the current context
    /// </param>
    /// 
    /// <param name="cancellationToken">
    ///     Represents the CancellationToken wihch will be used to facilitate the cancellation of operation.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous AddRange operation. 
    ///     The task result contains <see langword="true"/> if the source sequence contains 
    ///     any elements; otherwise, <see langword="false"/>
    /// </returns>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (filter != null)
            queryable = queryable.Where(filter);

        return await queryable.AnyAsync(cancellationToken);
    }

    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Delete</b> 
    ///     an <paramref name="entity"/> from connected database 
    ///     This method also calls methods to determine if the entity is valid for deleting.
    /// </summary>
    /// 
    /// <param name="entity">
    ///     Represents the entity with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    /// <param name="permanent">
    ///     Represents the boolean flag which will be used to determine whether to delete the record permanently
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous Delete operation. 
    ///     The task result contains the entity on which the operation was performed.
    /// </returns>
    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, permanent);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Delete</b> 
    ///     a list of <paramref name="entities"/> from connected database 
    ///     This method also calls methods to determine if the entities are valid for deleting.
    /// </summary>
    /// 
    /// <param name="entities">
    ///     Represents the list of entity with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    /// <param name="permanent">
    ///     Represents the boolean flag which will be used to determine whether to delete the record permanently
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous DeleteRange operation. 
    ///     The task result contains the list of entity on which the operation was performed.
    /// </returns>
    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, permanent);
        await Context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    ///     <b>Asynchronously</b> returns the first element of a sequence, 
    ///     or a default value if the sequence contains no element
    ///     Within the method, it can accept parameters to modify the query.
    /// </summary>
    /// 
    /// <param name="filter">
    ///     Represents the linq query which will modify the default query
    /// </param>
    /// 
    /// <param name="include">
    ///     Represents the linq query which will modify the query to include records of given related entities.
    /// </param>
    /// 
    /// <param name="withDeleted">
    ///     Represents the boolean flag which will determine whether to fetch soft deleted records
    /// </param>
    /// 
    /// <param name="enableTracking">
    ///     Represents the boolean flag which will determine whether to let EF Core to 
    ///     track the entity of the current context
    /// </param>
    /// 
    /// <param name="cancellationToken">
    ///     Represents the CancellationToken wihch will be used to facilitate the cancellation of operation.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous FirstOrDefault operation. 
    ///     The task result contains <see langword="default"/> ( <typeparamref name="TEntity"/> ) if
    ///     the source query result is <see langword="null"/>; otherwise, the first element in sequence.
    /// </returns>
    public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query
                                        (
                                            filter: filter, 
                                            include: include, 
                                            withDeleted: withDeleted, 
                                            enableTracking: enableTracking
                                        );
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    ///     <b>Asynchronously</b> returns the only element of a sequence, or a default value if the sequence is empty; 
    ///     this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// 
    /// <param name="filter">
    ///     Represents the linq query which will modify the default query
    /// </param>
    /// 
    /// <param name="include">
    ///     Represents the linq query which will modify the query to include records of given related entities.
    /// </param>
    /// 
    /// <param name="withDeleted">
    ///     Represents the boolean flag which will determine whether to fetch soft deleted records
    /// </param>
    /// 
    /// <param name="enableTracking">
    ///     Represents the boolean flag which will determine whether to let EF Core to 
    ///     track the entity of the current context
    /// </param>
    /// 
    /// <param name="cancellationToken">
    ///     Represents the CancellationToken wihch will be used to facilitate the cancellation of operation.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous SingleOrDefault operation. The task result contains the 
    ///     single element of the input sequence, or <see langword="default"/> ( <typeparamref name="TEntity"/> ) 
    ///     if the sequence contains no elements.
    /// </returns>
    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query
                                        (
                                            filter: filter,
                                            include: include,
                                            withDeleted: withDeleted,
                                            enableTracking: enableTracking
                                        );
        return await queryable.SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    ///     <b>Asynchronously</b> creates a <see cref="Paginate{T}"/> from an <see cref="IQueryable{T}" /> by enumerating it
    ///     asynchronously.
    /// </summary>
    /// 
    /// <param name="filter">
    ///     Represents the linq query which will modify the default query
    /// </param>
    /// 
    /// <param name="include">
    ///     Represents the linq query which will modify the query to include records of given related entities.
    /// </param>
    /// 
    /// <param name="orderBy">
    ///     Represents the linq query which will modify the query to sort the sequence with given key.
    /// </param>
    /// 
    /// <param name="index">
    ///     Represents the startin index which will be fetched data from paginated list
    /// </param>
    /// 
    /// <param name="size">
    ///     Represents the maximum number of data which will be fetched from paginated list
    /// </param>
    /// 
    /// <param name="withDeleted">
    ///     Represents the boolean flag which will determine whether to fetch soft deleted records
    /// </param>
    /// 
    /// <param name="enableTracking">
    ///     Represents the boolean flag which will determine whether to let EF Core to 
    ///     track the entity of the current context
    /// </param>
    /// 
    /// <param name="cancellationToken">
    ///     Represents the CancellationToken wihch will be used to facilitate the cancellation of operation.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous Paginate operation. 
    ///     The task result contains the list of paginated items of <typeparamref name="TEntity"/> 
    ///     that contains elements from the input sequence.
    /// </returns>
    public async Task<Paginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query
                                        (
                                            filter,
                                            include, 
                                            orderBy, 
                                            withDeleted, 
                                            enableTracking
                                        );
        return await queryable.PaginateAsync(index, size, cancellationToken);
    }

    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Update</b> 
    ///     an <paramref name="entity"/> on connected database while also assigning the update date of the record.
    ///     Update operation itself is not asynchronous, but the writing changes process is asynchronous.
    /// </summary>
    /// 
    /// <param name="entity">
    ///     Represents the entity with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous operation. 
    ///     The task result contains the entity on which the operation was performed.
    /// </returns>
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        Context.Update(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    ///     <b>Asynchronously</b> performs <see cref="EntityFrameworkCore"/> operations to <b>Update</b> 
    ///     a list of <paramref name="entities"/> on connected database while also assigning the update date of the record.
    ///     Update operation itself is not asynchronous, but the writing changes process is asynchronous.
    /// </summary>
    /// 
    /// <param name="entities">
    ///     Represents the list of entities with the identity type of <see cref="TEntityIdType"/> 
    ///     on which the operation will be performed.
    /// </param>
    /// 
    /// <returns>
    ///     A task that represents the asynchronous operation. 
    ///     The task result contains the list of entities on which the operation was performed.
    /// </returns>
    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
            entity.UpdatedDate = DateTime.UtcNow;
        Context.Update(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHasOneToOneRelation(entity);
            await setEntityAsSoftDeletedAsync(entity);
        }
        else
            Context.Remove(entity);
    }

    protected async Task SetEntityAsDeletedAsync(ICollection<TEntity> entities, bool permanent)
    {
        foreach (var entity in entities)
            await SetEntityAsDeletedAsync(entity, permanent);
    }

    private async Task setEntityAsSoftDeletedAsync(TEntity entity)
    {
        if (entity.DeletedDate.HasValue)
            return;
        entity.DeletedDate = DateTime.UtcNow;

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToListAsync();
                    if (navValue == null)
                        continue;
                }

                foreach (TEntity navValueItem in (IEnumerable)navValue)
                    await setEntityAsSoftDeletedAsync(navValueItem);
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = await GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefaultAsync();
                    if (navValue == null)
                        continue;
                }

                await setEntityAsSoftDeletedAsync((TEntity)navValue);
            }
        }

        Context.Update(entity);
    }
    #endregion

    #region Sync
    public TEntity Add(TEntity entity)
    {
        entity.CreatedDate = DateTime.Now;
        Context.Add(entity);
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
            entity.CreatedDate = DateTime.UtcNow;
        Context.Add(entities);
        Context.SaveChanges();
        return entities;
    }

    public bool? Any(Expression<Func<TEntity, bool>>? filter = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (filter != null)
            queryable = queryable.Where(filter);

        return queryable.Any();
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        SetEntityAsDeleted(entity, permanent);
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        SetEntityAsDeleted(entities, permanent);
        Context.SaveChanges();
        return entities;
    }

    public TEntity? GetFirst(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query
                                        (
                                            filter: filter,
                                            include: include,
                                            withDeleted: withDeleted,
                                            enableTracking: enableTracking
                                        );
        return queryable.FirstOrDefault();
    }
    
    public TEntity? GetSingle(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query
                                        (
                                            filter: filter,
                                            include: include,
                                            withDeleted: withDeleted,
                                            enableTracking: enableTracking
                                        );
        return queryable.SingleOrDefault();
    }

    public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = Query();

        if (filter != null)
            queryable = queryable.Where(filter);
        if (include != null)
            queryable = include(queryable);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (orderBy != null)
            return orderBy(queryable).Paginate(index, size);

        return queryable.Paginate(index, size);
    }

    public TEntity Update(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        Context.Update(entity);
        Context.SaveChanges();
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        foreach (var entity in entities)
            entity.UpdatedDate = DateTime.UtcNow;
        Context.Update(entities);
        Context.SaveChanges();
        return entities;
    }

    protected void SetEntityAsDeleted(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHasOneToOneRelation(entity);
            setEntityAsSoftDeleted(entity);
        }
        else
            Context.Remove(entity);
    }

    protected void SetEntityAsDeleted(ICollection<TEntity> entities, bool permanent)
    {
        foreach (var entity in entities)
            SetEntityAsDeleted(entity, permanent);
    }

    private void setEntityAsSoftDeleted(TEntity entity)
    {
        if (entity.DeletedDate.HasValue)
            return;
        entity.DeletedDate = DateTime.UtcNow;

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            if (navigation.IsCollection)
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Query();
                    navValue = GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType()).ToList();
                    if (navValue == null)
                        continue;
                }

                foreach (TEntity navValueItem in (IEnumerable)navValue)
                    setEntityAsSoftDeleted(navValueItem);
            }
            else
            {
                if (navValue == null)
                {
                    IQueryable query = Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Query();
                    navValue = GetRelationLoaderQuery(query, navigationPropertyType: navigation.PropertyInfo.GetType())
                        .FirstOrDefault();
                    if (navValue == null)
                        continue;
                }

                setEntityAsSoftDeleted((TEntity)navValue);
            }
        }

        Context.Update(entity);
    }
    #endregion

    public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool withDeleted = false, bool enableTracking = true) 
    {
        IQueryable<TEntity> queryable = Context.Set<TEntity>();

        if (filter != null)
            queryable = queryable.Where(filter);
        if (include != null)
            queryable = include(queryable);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (orderBy != null)
            queryable = orderBy(queryable);

        return queryable;
    }
    protected void CheckHasEntityHasOneToOneRelation(TEntity entity)
    {
        bool hasEntityHaveOneToOneRelation =
            Context
            .Entry(entity)
            .Metadata.GetForeignKeys()
            .All(x => x.DependentToPrincipal?.IsCollection == true
                   || x.PrincipalToDependent?.IsCollection == true
                   || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException("Entity has one-to-one relationship. Soft Delete causes problems " +
                "if you try to create entry again by the same foreign key");
    }

    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;
        return queryProviderQuery.Where(x => !((TEntity)x).DeletedDate.HasValue);
    }
}
