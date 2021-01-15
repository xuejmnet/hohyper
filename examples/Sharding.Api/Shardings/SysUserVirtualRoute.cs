using System;
using System.Linq.Expressions;
using System.Text;
using HoHyper.ShardingCore.VirtualRoutes;
using HoHyper.ShardingCore.VirtualRoutes.Abstractions;
using Microsoft.Extensions.Logging;
using Sharding.Api.Domain.Entities;

namespace Sharding.Api.Shardings
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 14 January 2021 15:39:27
* @Email: 326308290@qq.com
*/
    public class SysUserVirtualRoute : AbstractShardingOperatorVirtualRoute<SysUser, string>
    {
        private readonly ILogger<SysUserVirtualRoute> _logger;
        private int _mod = 3;

        public SysUserVirtualRoute(ILogger<SysUserVirtualRoute> logger)
        {
            _logger = logger;
        }

        protected override string ConvertToShardingKey(object shardingKey)
        {
            return shardingKey.ToString();
        }

        public override string ShardingKeyToTail(object shardingKey)
        {
            var shardingKeyStr = ConvertToShardingKey(shardingKey);
            var bytes = Encoding.Default.GetBytes(shardingKeyStr);
            return Math.Abs(BitConverter.ToInt32(bytes, 0) % _mod).ToString();
        }

        protected override Expression<Func<string, bool>> GetRouteToFilter(string shardingKey, ShardingOperatorEnum shardingOperator)
        {
            var t = ShardingKeyToTail(shardingKey);
            switch (shardingOperator)
            {
                case ShardingOperatorEnum.Equal: return tail => tail == t;
                case ShardingOperatorEnum.NotEqual: return tail => tail != t;
                default:
                {
                    _logger.LogWarning($"shardingOperator is not equal scan all table tail");
                    return tail => true;
                }
            }
        }
    }
}