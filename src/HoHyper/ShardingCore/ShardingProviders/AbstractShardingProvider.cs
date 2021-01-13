using System;
using System.Collections.Generic;
using System.Linq;
using HoHyper.Exceptions;
using HoHyper.ShardingCore.VirtualRoutes;

namespace HoHyper.ShardingCore.ShardingProviders
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 21 December 2020 09:46:07
* @Email: 326308290@qq.com
*/
    public abstract class AbstractShardingProvider<T>:IShardingProvider<T>where T:class,IShardingEntity
    {
        private readonly IVirtualRoute<T> _virtualRoute;
        public Type ShardingEntityType => typeof(T);

        protected AbstractShardingProvider(IEnumerable<IVirtualRoute> virtualRoutes)
        {
            _virtualRoute = (IVirtualRoute<T>)virtualRoutes.FirstOrDefault(o=>o.ShardingEntityType==ShardingEntityType)??throw new VirtualRouteNotFoundException($"{ShardingEntityType}");
        }
        IVirtualRoute IShardingProvider.GetShardingRoute()
        {
            return GetShardingRoute();
        }
        /// <summary>
        /// 获取分表路由
        /// </summary>
        /// <returns></returns>
        public IVirtualRoute<T> GetShardingRoute()
        {
            return _virtualRoute;
        }
        /// <summary>
        /// 获取所有的物理表后缀
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetExistsPhysicTableTails();
    }
}