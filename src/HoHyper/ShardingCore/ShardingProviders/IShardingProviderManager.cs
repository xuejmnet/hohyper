using System;

namespace HoHyper.ShardingCore.ShardingProviders
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 16 December 2020 10:48:37
* @Email: 326308290@qq.com
*/
    public interface IShardingProviderManager
    {
        /// <summary>
        /// 获取分表所属的分表所有者
        /// </summary>
        /// <param name="shardingEntityType"></param>
        /// <returns></returns>
        IShardingProvider GetShardingOwner(Type shardingEntityType);
        /// <summary>
        /// 获取分表所属的分表所有者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IShardingProvider<T> GetShardingOwner<T>() where T:class,IShardingEntity;

    }
}