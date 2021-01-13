using HoHyper.DbContexts;
using HoHyper.DbContexts.ShardingDbContexts;
using HoHyper.EFCores;
using HoHyper.Extensions;
using HoHyper.ShardingCore.VirtualTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace HoHyper.SqlServer
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 29 December 2020 15:22:50
* @Email: 326308290@qq.com
*/
    public class ShardingSqlServerParallelDbContextFactory : IShardingParallelDbContextFactory
    {
        private readonly IVirtualTableManager _virtualTableManager;
        private readonly SqlServerOptions _sqlServerOptions;

        public ShardingSqlServerParallelDbContextFactory(IVirtualTableManager virtualTableManager, SqlServerOptions sqlServerOptions)
        {
            _virtualTableManager = virtualTableManager;
            _sqlServerOptions = sqlServerOptions;
        }

        public ShardingDbContext Create(string tail)
        {
            var virtualTableConfigs = _virtualTableManager.GetAllVirtualTables().GetVirtualTableDbContextConfigs();
            var shardingDbContextOptions = new ShardingDbContextOptions(CreateOptions(), tail, virtualTableConfigs);
            return new ShardingDbContext(shardingDbContextOptions);
        }

        private DbContextOptions CreateOptions()
        {
            return new DbContextOptionsBuilder()
                .UseSqlServer(_sqlServerOptions.ConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .ReplaceService<IQueryCompiler, ShardingQueryCompiler>()
                .ReplaceService<IModelCacheKeyFactory, ShardingModelCacheKeyFactory>()
                .UseShardingSqlServerQuerySqlGenerator()
                .Options;
        }
    }
}