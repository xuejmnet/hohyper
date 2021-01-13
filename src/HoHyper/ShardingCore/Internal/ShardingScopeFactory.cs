using HoHyper.ShardingCore.Internal.ShardingAccessors;

namespace HoHyper.ShardingCore.Internal
{
/*
* @Author: xjm
* @Description:
* @Date: Wednesday, 23 December 2020 08:11:06
* @Email: 326308290@qq.com
*/
    /// <summary>
    /// 分表查询环境创建
    /// </summary>
    public class ShardingScopeFactory : IShardingScopeFactory
    {
        private readonly IShardingAccessor _shardingAccessor;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="shardingAccessor"></param>
        public ShardingScopeFactory(IShardingAccessor shardingAccessor)
        {
            _shardingAccessor = shardingAccessor;
        }
        /// <summary>
        /// 创建scope
        /// </summary>
        /// <returns></returns>
        public ShardingScope CreateScope()
        {
            return new ShardingScope(_shardingAccessor);
        }
    }
}