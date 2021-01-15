using System;
using System.Collections.Generic;
using HoHyper.ShardingCore.ShardingProviders;
using HoHyper.ShardingCore.VirtualRoutes;
using Sharding.Api.Domain.Entities;

namespace Sharding.Api.Shardings
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 14 January 2021 15:44:01
* @Email: 326308290@qq.com
*/
    public class SysUserShardingProvider:AbstractShardingProvider<SysUser>
    {
        public SysUserShardingProvider(IEnumerable<IVirtualRoute> virtualRoutes) : base(virtualRoutes)
        {
        }

        public override List<string> GetExistsPhysicTableTails()
        {
            return new() { "0","1","2"};
        }
    }
}