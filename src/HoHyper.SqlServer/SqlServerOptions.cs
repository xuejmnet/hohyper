using System.Collections.Generic;
using HoHyper.Extensions;
using HoHyper.ShardingCore;
using HoHyper.ShardingCore.ShardingProviders;
using HoHyper.ShardingCore.VirtualRoutes;

namespace HoHyper.SqlServer
{
/*
* @Author: xjm
* @Description:
* @Date: 2020年4月7日 8:34:04
* @Email: 326308290@qq.com
*/
    public class SqlServerOptions
    {
        public LinkedList<ShardingEntry> ShardingEntries=new LinkedList<ShardingEntry>();
        public  string ConnectionString { get; set; }

        public void AddSharding<TOwner,TRoute>()where TOwner:IShardingProvider
        where TRoute:IVirtualRoute
        {
            ShardingEntries.AddLast(new ShardingEntry(typeof(TOwner), typeof(TRoute)));
        }

        public bool HasSharding => ShardingEntries.IsNotEmpty();
    }
}