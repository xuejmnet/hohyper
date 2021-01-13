using HoHyper.DbContexts.ShardingDbContexts;

namespace HoHyper.DbContexts
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 24 December 2020 08:22:23
* @Email: 326308290@qq.com
*/
    public interface IShardingDbContextFactory
    {
        ShardingDbContext Create(ShardingDbContextOptions shardingDbContextOptions);
    }
}