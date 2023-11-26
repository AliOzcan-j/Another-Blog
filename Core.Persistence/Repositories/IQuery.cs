using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories;

public interface IQuery<T>
{
    IQueryable<T> Query
        (
            Expression<Func<T, bool>>? filter = null, 
            Func<IQueryable<T>, IIncludableQueryable<T,object>>? include = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool withDeleted = false, 
            bool enableTracking = true
        );
}