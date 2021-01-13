using System;
using System.Collections.Generic;
using System.Linq;
using HoHyper.Exceptions;

namespace HoHyper.ShardingCore.ShardingProviders
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 16 December 2020 13:07:37
* @Email: 326308290@qq.com
*/
    public class ShardingProviderManager : IShardingProviderManager
    {
        private readonly IEnumerable<IShardingProvider> _shardingOwners;

        public ShardingProviderManager(IEnumerable<IShardingProvider> shardingOwners)
        {
            _shardingOwners = shardingOwners;
        }
        public IShardingProvider GetShardingOwner(Type shardingEntityType)
        {
            var shardingOwner = _shardingOwners.FirstOrDefault(o => o.ShardingEntityType == shardingEntityType);
            return shardingOwner ?? throw new ShardingOwnerNotFoundException($"{shardingEntityType}");
        }

        public IShardingProvider<T> GetShardingOwner<T>() where T : class, IShardingEntity
        {
            return (IShardingProvider<T>)GetShardingOwner(typeof(T));
        }
    }
}