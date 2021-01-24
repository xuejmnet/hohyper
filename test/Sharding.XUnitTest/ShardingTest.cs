using System;
using System.Linq;
using System.Threading.Tasks;
using HoHyper.DbContexts.VirtualDbContexts;
using HoHyper.Extensions;
using Sharding.XUnitTest.Domain.Entities;
using Xunit;

namespace Sharding.XUnitTest
{
/*
* @Author: xjm
* @Description:
* @Date: Friday, 15 January 2021 17:22:10
* @Email: 326308290@qq.com
*/
    public class ShardingTest
    {
        private readonly IVirtualDbContext _virtualDbContext;

        public ShardingTest(IVirtualDbContext virtualDbContext)
        {
            _virtualDbContext = virtualDbContext;
        }

        [Fact]
        public async Task ToList1()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().ToShardingListAsync();
            Assert.Equal(100,mods.Count);
            var ranges=await _virtualDbContext.Set<SysUserRange>().ToShardingListAsync();
            Assert.Equal(1000,ranges.Count);
        }
        [Fact]
        public async Task ToList2()
        {
            var ids = new[] {"1", "2", "3", "4"};
            var sysUserMods=await _virtualDbContext.Set<SysUserMod>().Where(o=>ids.Contains(o.Id)).ToShardingListAsync();
            var sysUserRanges=await _virtualDbContext.Set<SysUserRange>().Where(o=>ids.Contains(o.Id)).ToShardingListAsync();
            foreach (var id in ids)
            {
                Assert.Contains(sysUserMods, o =>o.Id==id);
                Assert.Contains(sysUserRanges, o =>o.Id==id);
            }
        }
        [Fact]
        public async Task ToList3()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="3").ToShardingListAsync();
            Assert.Single(mods);
            Assert.Equal("3",mods[0].Id);
            var ranges=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Id=="3").ToShardingListAsync();
            Assert.Single(ranges);
            Assert.Equal("3",ranges[0].Id);
        }
        [Fact]
        public async Task ToList4()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id!="3").ToShardingListAsync();
            Assert.Equal(99,mods.Count);
            Assert.DoesNotContain(mods,o=>o.Id=="3");
            var ranges=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Id!="3").ToShardingListAsync();
            Assert.Equal(999,ranges.Count);
            Assert.DoesNotContain(ranges,o=>o.Id=="3");
        }
        [Fact]
        public async Task ToList5()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_3").ToShardingListAsync();
            Assert.Single(mods);
            Assert.Equal("3",mods[0].Id);
            var ranges=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Name=="name_range_3").ToShardingListAsync();
            Assert.Single(ranges);
            Assert.Equal("3",ranges[0].Id);
        }
        [Fact]
        public async Task ToList6()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="1001").ToShardingListAsync();
            Assert.Empty(mods);
            var ranges=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Id=="1001").ToShardingListAsync();
            Assert.Empty(ranges);
        }
        [Fact]
        public async Task ToList7()
        {
            var mods=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_1001").ToShardingListAsync();
            Assert.Empty(mods);
            var ranges=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Name=="name_range_1001").ToShardingListAsync();
            Assert.Empty(ranges);
        }
        [Fact]
        public async Task FirstOrDefault1()
        {
            var sysUserMod=await _virtualDbContext.Set<SysUserMod>().OrderBy(o=>o.Id).ShardingFirstOrDefaultAsync();
            Assert.True(sysUserMod!=null&&sysUserMod.Id=="1");
            var sysUserRange=await _virtualDbContext.Set<SysUserRange>().OrderBy(o=>o.Id).ShardingFirstOrDefaultAsync();
            Assert.True(sysUserRange!=null&&sysUserRange.Id=="1");
        }
        [Fact]
        public async Task FirstOrDefault2()
        {
            var sysUserMod=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="1").ShardingFirstOrDefaultAsync();
            Assert.True(sysUserMod!=null&&sysUserMod.Id=="1");
            var sysUserRange=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Id=="1").ShardingFirstOrDefaultAsync();
            Assert.True(sysUserRange!=null&&sysUserRange.Id=="1");
        }
        [Fact]
        public async Task FirstOrDefault3()
        {
            var sysUserMod=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_2").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUserMod);
            Assert.Equal("2",sysUserMod.Id);
            var sysUserRange=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Name=="name_range_2").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUserRange);
            Assert.Equal("2",sysUserRange.Id);
        }
        [Fact]
        public async Task FirstOrDefault4()
        {
            var sysUserMod=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id!="1").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUserMod);
            Assert.True(sysUserMod.Id!="1");
            var sysUserRange=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Id!="1").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUserRange);
            Assert.True(sysUserRange.Id!="1");
        }
        [Fact]
        public async Task FirstOrDefault5()
        {
            var sysUserMod=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_1001").ShardingFirstOrDefaultAsync();
            Assert.Null(sysUserMod);
            var sysUserRange=await _virtualDbContext.Set<SysUserRange>().Where(o=>o.Name=="name_range_1001").ShardingFirstOrDefaultAsync();
            Assert.Null(sysUserRange);
        }
    }
}