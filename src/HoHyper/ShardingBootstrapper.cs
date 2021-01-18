using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging;

namespace HoHyper
{
/*
* @Author: xjm
* @Description:
* @Date: Monday, 21 December 2020 09:10:07
* @Email: 326308290@qq.com
*/
    public class ShardingBootstrapper 
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IVirtualTableManager _virtualTableManager;
        private readonly IShardingTableCreator _tableCreator;
        private readonly ILogger<ShardingBootstrapper> _logger;
        private readonly IShardingDbContextFactory _shardingDbContextFactory;
        private readonly HoHyperConfig _hoHyperConfig;

        public ShardingBootstrapper(IServiceProvider serviceProvider, IVirtualTableManager virtualTableManager
            , IShardingTableCreator tableCreator, ILogger<ShardingBootstrapper> logger,
            IShardingDbContextFactory shardingDbContextFactory, HoHyperConfig hoHyperConfig)
        {
            ShardingContainer.SetServices(serviceProvider);
            _serviceProvider = serviceProvider;
            _virtualTableManager = virtualTableManager;
            _tableCreator = tableCreator;
            _logger = logger;
            _shardingDbContextFactory = shardingDbContextFactory;
            _hoHyperConfig = hoHyperConfig;
        }

        public void Start()
        {
            EnsureCreated();
            var virtualTables = _virtualTableManager.GetAllVirtualTables();
            using var scope = _serviceProvider.CreateScope();
            var dbContextOptionsProvider = scope.ServiceProvider.GetService<IDbContextOptionsProvider>();
            using var context = _shardingDbContextFactory.Create(new ShardingDbContextOptions(dbContextOptionsProvider.GetDbContextOptions(), string.Empty, virtualTables.GetVirtualTableDbContextConfigs()));

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

        public void EnsureCreated()
        {
            if (_hoHyperConfig.EnsureCreated)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContextOptionsProvider = scope.ServiceProvider.GetService<IDbContextOptionsProvider>();
                using var context = _shardingDbContextFactory.Create(new ShardingDbContextOptions(dbContextOptionsProvider.GetDbContextOptions(), string.Empty, new List<VirtualTableDbContextConfig>(), true));
                context.Database.EnsureCreated();
            }
        }

        private void CreateDataTable(IVirtualTable virtualTable)
        {
            var shardingConfig = virtualTable.ShardingConfig;
            foreach (var tail in virtualTable.GetTaleAllTails())
            {
                if (shardingConfig.AutoCreateTable)
                {
                    try
                    {
                        _tableCreator.CreateTable(virtualTable.EntityType, tail);
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning($"table :{virtualTable.GetOriginalTableName()}{shardingConfig.TailPrefix}{tail} will created");
                    }
                }

                //添加物理表
                virtualTable.AddPhysicTable(new DefaultPhysicTable(virtualTable.GetOriginalTableName(), virtualTable.ShardingConfig.TailPrefix, tail, virtualTable.EntityType));
            }
        }
    }
}