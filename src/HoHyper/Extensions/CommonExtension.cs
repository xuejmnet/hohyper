using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HoHyper.DbContexts.ShardingDbContexts;
using HoHyper.ShardingCore;
using HoHyper.ShardingCore.VirtualTables;
using HoHyper.Utils;

namespace HoHyper.Extensions
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 01 January 2021 19:51:44
* @Email: 326308290@qq.com
*/
    public static class CommonExtension
    {
        /// <summary>
        /// 是否基继承至ShardingEntity
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static bool IsShardingEntity(this Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));
            return typeof(IShardingEntity).IsAssignableFrom(entityType);
        }

        /// <summary>
        /// 是否基继承至ShardingEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool IsShardingEntity(this object entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return typeof(IShardingEntity).IsAssignableFrom(entity.GetType());
        }
        /// <summary>
        /// 虚拟表转换成对应的db配置
        /// </summary>
        /// <param name="virtualTables"></param>
        /// <returns></returns>
        public static List<VirtualTableDbContextConfig> GetVirtualTableDbContextConfigs(this List<IVirtualTable> virtualTables)
        {
            return virtualTables.Select(o => new VirtualTableDbContextConfig(o.EntityType, o.GetOriginalTableName(), o.ShardingConfig.TailPrefix)).ToList();
        }
        /// <summary>
        /// 是否是集合contains方法
        /// </summary>
        /// <param name="express"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool IsEnumerableContains(this MethodCallExpression express, string methodName)
        {
            return  express.Method.DeclaringType.Namespace.IsIn("System.Linq", "System.Collections.Generic") && methodName == nameof(IList.Contains);
        }
        public static ISet<Type> ParseQueryableRoute(this IQueryable queryable)
        {
            return ShardingKeyUtil.GetShardingEntitiesFilter(queryable);
        }
    }
}