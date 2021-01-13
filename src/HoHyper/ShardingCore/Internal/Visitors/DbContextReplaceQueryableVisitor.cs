using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HoHyper.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace HoHyper.ShardingCore.Internal.Visitors
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 13 January 2021 16:32:27
* @Email: 326308290@qq.com
*/
    internal class DbContextReplaceQueryableVisitor : ExpressionVisitor
    {
        private readonly DbContext _dbContext;
        public IQueryable Source;

        public DbContextReplaceQueryableVisitor(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is IQueryable queryable)
            {
                var dbContextDependencies = typeof(DbContext).GetTypePropertyValue(_dbContext, "DbContextDependencies") as IDbContextDependencies;
                var targetIQ = (IQueryable)((IDbSetCache)_dbContext).GetOrAddSet(dbContextDependencies.SetSource, queryable.ElementType);
                var newQueryable=targetIQ.Provider.CreateQuery((Expression) Expression.Call((Expression) null, typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethod("AsNoTracking").MakeGenericMethod(queryable.ElementType), targetIQ.Expression));
                if (Source == null)
                {
                    Source = newQueryable;
                }
                return Expression.Constant(newQueryable);
            }

            return base.VisitConstant(node);
        }
    }
}