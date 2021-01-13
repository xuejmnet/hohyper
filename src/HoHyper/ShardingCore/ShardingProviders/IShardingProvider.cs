using System;
using System.Collections.Generic;
using HoHyper.ShardingCore.VirtualRoutes;

namespace HoHyper.ShardingCore.ShardingProviders
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 21 December 2020 09:16:29
* @Email: 326308290@qq.com
*/
    public interface IShardingProvider
    {
        /// <summary>
        /// 分表类型
        /// </summary>
        Type ShardingEntityType { get; }
        /// <summary>
        /// 分表路由 get sharding route
        /// </summary>
        /// <returns></returns>
        IVirtualRoute GetShardingRoute();
        /// <summary>
        /// 获取已存在的物理表
        /// </summary>
        /// <returns></returns>
        List<string> GetExistsPhysicTableTails();
    }

    public interface IShardingProvider<T>:IShardingProvider where T : class, IShardingEntity
    {
         new IVirtualRoute<T> GetShardingRoute();
    }
}