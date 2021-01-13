using System;
using System.Threading;
using System.Threading.Tasks;
using HoHyper.DbContexts;
using HoHyper.DbContexts.ShardingDbContexts;
using HoHyper.DbContexts.VirtualDbContexts;
using HoHyper.Extensions;
using HoHyper.ShardingCore.PhysicTables;
using HoHyper.ShardingCore.VirtualTables;
using HoHyper.TableCreator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HoHyper
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 21 December 2020 09:10:07
* @Email: 326308290@qq.com
*/
    public class ShardingBootstrapper:BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IVirtualTableManager _virtualTableManager;
        private readonly IShardingTableCreator _tableCreator;
        private readonly ILogger<ShardingBootstrapper> _logger;
        private readonly IShardingDbContextFactory _shardingDbContextFactory;

        public ShardingBootstrapper(IServiceProvider serviceProvider,IVirtualTableManager virtualTableManager
            ,IShardingTableCreator tableCreator,ILogger<ShardingBootstrapper> logger,
            IShardingDbContextFactory shardingDbContextFactory)
        {
            _serviceProvider = serviceProvider;
            _virtualTableManager = virtualTableManager;
            _tableCreator = tableCreator;
            _logger = logger;
            _shardingDbContextFactory = shardingDbContextFactory;
        }

        public void Start()
        {
            var virtualTables = _virtualTableManager.GetAllVirtualTables();
            using var scope = _serviceProvider.CreateScope();
            var dbContextOptionsProvider = scope.ServiceProvider.GetService<IDbContextOptionsProvider>();
            using var context = _shardingDbContextFactory.Create(new ShardingDbContextOptions(dbContextOptionsProvider.GetDbContextOptions(),string.Empty,virtualTables.GetVirtualTableDbContextConfigs()));
            foreach (var virtualTable in virtualTables)
            {
                //获取ShardingEntity的实际表名
#if !EFCORE2
                var tableName = context.Model.FindEntityType(virtualTable.EntityType).GetTableName();
#endif
#if EFCORE2
                var tableName = context.Model.FindEntityType(virtualTable.EntityType).Relational().TableName;  
#endif
                virtualTable.SetOriginalTableName(tableName);
                CreateDataTable(virtualTable);
            }
        }

        private void CreateDataTable(IVirtualTable virtualTable)
        {
            var shardingConfig = virtualTable.ShardingConfig;
            foreach (var tail in virtualTable.GetShardingOwnerTails())
            {
                if (shardingConfig.AutoCreateTable)
                {
                    _tableCreator.CreateTable(virtualTable.EntityType, tail);
                }
                //添加物理表
                virtualTable.AddPhysicTable(new DefaultPhysicTable(virtualTable.GetOriginalTableName(),virtualTable.ShardingConfig.TailPrefix,tail,virtualTable.EntityType));
            }
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Start();
            return Task.CompletedTask;
        }
    }
}