using System;

namespace HoHyper.ShardingCore
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 21 December 2020 10:19:48
* @Email: 326308290@qq.com
*/
    public class ShardingEntry
    {
        public ShardingEntry(Type shardingOwner, Type shardingRoute)
        {
            ShardingOwner = shardingOwner;
            ShardingRoute = shardingRoute;
        }

        public Type ShardingOwner { get; }
        public Type ShardingRoute { get; }
    }
}