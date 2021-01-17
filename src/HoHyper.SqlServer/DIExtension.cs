using System;
using System.Collections.Generic;
using System.Linq;
using HoHyper.DbContexts;
using HoHyper.DbContexts.VirtualDbContexts;
using HoHyper.Extensions;
using HoHyper.Helpers;
using HoHyper.ShardingCore.Internal;
using HoHyper.ShardingCore.ShardingAccessors;
using HoHyper.ShardingCore.VirtualRoutes;
using HoHyper.ShardingCore.VirtualTables;
using HoHyper.SqlServer.EFCores;
using HoHyper.TableCreator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

#if EFCORE2
using Microsoft.EntityFrameworkCore.Query.Sql;
#endif

namespace HoHyper.SqlServer
{
    /*
    * @Author: xjm
    * @Description: 
    * @Date: 2020年4月7日 9:30:18
    * @Email: 326308290@qq.com
    */
    public static class DIExtension
    {
        public static IServiceCollection AddShardingSqlServer(this IServiceCollection services, Action<SqlServerOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException($"AddScfSqlServerProvider 参数不能为空:{nameof(configure)}");
           
            var options = new SqlServerOptions();
            configure(options);
            services.AddSingleton(options);

            services.AddScoped<IVirtualDbContext, VirtualDbContext>();
            services.AddScoped<IDbContextOptionsProvider, SqlServerDbContextOptionsProvider>();
            services.AddSingleton<IShardingDbContextFactory, ShardingDbContextFactory>();
            services.AddSingleton<IShardingTableCreator, ShardingTableCreator>();
            services.AddSingleton<IVirtualTableManager, OneDbVirtualTableManager>();
            services.AddSingleton(typeof(IVirtualTable<>), typeof(OneDbVirtualTable<>));
            services.AddSingleton<IShardingAccessor, ShardingAccessor>();
            services.AddSingleton<IShardingScopeFactory, ShardingScopeFactory>();
            services.AddSingleton<IShardingParallelDbContextFactory, ShardingSqlServerParallelDbContextFactory>();
            if (options.HasSharding)
            {
                foreach (var shardingRoute in options.ShardingRoutes)
                {
                    var genericVirtualRoute = shardingRoute.GetInterfaces().FirstOrDefault(it => it.IsInterface && it.IsGenericType && it.GetGenericTypeDefinition() == typeof(IVirtualRoute<>)
                                                                                                && it.GetGenericArguments().Any());
                    if (genericVirtualRoute == null)
                        throw new ArgumentException("add sharding route type error not assignable from IVirtualRoute<>.");
                    var shardingEntity=genericVirtualRoute.GetGenericArguments()[0];
                    if(!shardingEntity.IsShardingEntity())
                        throw new ArgumentException("add sharding route type error generic arguments first not assignable from IShardingEntity.");
                    Type genericType = typeof(IVirtualRoute<>);
                    Type interfaceType = genericType.MakeGenericType(shardingEntity);
                    services.AddSingleton(interfaceType, shardingRoute);
                }
            }

            var hoHyperConfig = new HoHyperConfig();
            hoHyperConfig.EnsureCreated = options.EnsureCreated;
            services.AddSingleton(sp => hoHyperConfig);
            services.AddHostedService<ShardingBootstrapper>();
            return services;
        }

        public static DbContextOptionsBuilder UseShardingSqlServerQuerySqlGenerator(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, ShardingSqlServerQuerySqlGeneratorFactory>();
            return optionsBuilder;
        }
    }
}