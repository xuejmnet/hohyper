using System;
using System.Linq;
using System.Text;
using HoHyper.Helpers;
using HoHyper.ShardingCore;
using HoHyper.ShardingCore.VirtualRoutes.Abstractions;

namespace HoHyper.VirtualRoutes
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 21 January 2021 15:03:06
* @Email: 326308290@qq.com
*/
    public abstract class AbstractSimpleShardingModVirtualRoute<T,TKey>: AbstractShardingOperatorVirtualRoute<T, TKey> where T:class,IShardingEntity
    {
        protected readonly int Mod;
        protected AbstractSimpleShardingModVirtualRoute(int mod)
        {
            Mod = mod;
        }

        public override string ShardingKeyToTail(object shardingKey)
        {
            var shardingKeyStr = ConvertToShardingKey(shardingKey).ToString();
            return Math.Abs(HoHyperHelper.GetStringHashCode(shardingKeyStr) % Mod).ToString();
        }

    }
}