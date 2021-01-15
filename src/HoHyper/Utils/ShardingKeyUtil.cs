using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HoHyper.Extensions;
using HoHyper.ShardingCore;
using HoHyper.ShardingCore.Internal.Visitors;
using HoHyper.ShardingCore.VirtualRoutes;
using HoHyper.ShardingCore.VirtualTables;

namespace HoHyper.Utils
{
/*
* @Author: xjm
* @Description:
* @Date: Saturday, 19 December 2020 20:20:29
* @Email: 326308290@qq.com
*/
    public class ShardingKeyUtil
    {
        private static readonly ConcurrentDictionary<Type, ShardingEntityConfig> _caches = new ConcurrentDictionary<Type, ShardingEntityConfig>();

        private ShardingKeyUtil()
        {
        }

        public static ShardingEntityConfig Parse(Type entityType)
        {
            if (!typeof(IShardingEntity).IsAssignableFrom(entityType))
                throw new NotSupportedException(entityType.ToString());
            if (_caches.TryGetValue(entityType, out var shardingEntityConfig))
            {
                return shardingEntityConfig;
            }

            PropertyInfo[] shardingProperties = entityType.GetProperties();
            foreach (var shardingProperty in shardingProperties)
            {
                var attribbutes = shardingProperty.GetCustomAttributes(true);
                if (attribbutes.FirstOrDefault(x => x.GetType() == typeof(ShardingKeyAttribute)) is ShardingKeyAttribute shardingKeyAttribute)
                {
                    if (shardingEntityConfig != null)
                        throw new ArgumentException($"{entityType}存在多个[ShardingKeyAttribute]");
                    shardingEntityConfig = new ShardingEntityConfig()
                    {
                        ShardingEntityType = entityType,
                        ShardingField = shardingProperty.Name,
                        AutoCreateTable = shardingKeyAttribute.AutoCreateTableOnStart,
                        TailPrefix = shardingKeyAttribute.TailPrefix
                    };
                    _caches.TryAdd(entityType, shardingEntityConfig);
                }
            }

            return shardingEntityConfig;
        }

        public static Func<string, bool> GetRouteObjectOperatorFilter<TKey>(IQueryable queryable, ShardingEntityConfig shardingConfig, Func<object, TKey> shardingKeyConvert, Func<TKey, ShardingOperatorEnum, Expression<Func<string, bool>>> keyToTailExpression)
        {
            QueryableRouteDiscoverVisitor<TKey> visitor = new QueryableRouteDiscoverVisitor<TKey>(shardingConfig, shardingKeyConvert, keyToTailExpression);

            visitor.Visit(queryable.Expression);

            return visitor.GetStringFilterTail();
        }


        public static ISet<Type> GetShardingEntitiesFilter(IQueryable queryable)
        {
            ShardingEntitiesVisitor visitor = new ShardingEntitiesVisitor();

            visitor.Visit(queryable.Expression);

            return visitor.GetShardingEntities();
        }

    }
}