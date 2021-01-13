using System;
using System.Collections.Generic;
using System.Linq;
using HoHyper.ShardingCore.PhysicTables;

namespace HoHyper.ShardingCore.VirtualRoutes
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 18 December 2020 13:59:36
* @Email: 326308290@qq.com
*/
    /// <summary>
    /// 
    /// </summary>
    public interface IVirtualRoute
    {
        Type ShardingEntityType { get; }
        string ShardingKeyToTail(object shardingKey);

        /// <summary>
        /// 根据查询条件路由返回物理表
        /// </summary>
        /// <param name="allPhysicTables"></param>
        /// <param name="queryable"></param>
        /// <returns></returns>
        List<IPhysicTable> RouteWithWhere(List<IPhysicTable> allPhysicTables,IQueryable queryable);

        /// <summary>
        /// 根据值进行路由
        /// </summary>
        /// <param name="allPhysicTables"></param>
        /// <param name="shardingKeyValue"></param>
        /// <returns></returns>
        IPhysicTable RouteWithValue(List<IPhysicTable> allPhysicTables, object shardingKeyValue);
    }

    public interface IVirtualRoute<T> : IVirtualRoute where T : class, IShardingEntity
    {
    }
}