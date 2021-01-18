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
            var list=await _virtualDbContext.Set<SysUserMod>().ToShardingListAsync();
            Assert.Equal(100,list.Count);
        }
        [Fact]
        public async Task ToList2()
        {
            var ids = new[] {"1", "2", "3", "4"};
            var sysUsers=await _virtualDbContext.Set<SysUserMod>().Where(o=>ids.Contains(o.Id)).ToShardingListAsync();
            foreach (var id in ids)
            {
                Assert.Contains(sysUsers, o =>o.Id==id);
            }
        }
        [Fact]
        public async Task ToList3()
        {
            var list=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="3").ToShardingListAsync();
            Assert.Single(list);
            Assert.Equal("3",list[0].Id);
        }
        [Fact]
        public async Task ToList4()
        {
            var list=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id!="3").ToShardingListAsync();
            Assert.Equal(99,list.Count);
            Assert.DoesNotContain(list,o=>o.Id=="3");
        }
        [Fact]
        public async Task ToList5()
        {
            var list=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_3").ToShardingListAsync();
            Assert.Single(list);
            Assert.Equal("3",list[0].Id);
        }
        [Fact]
        public async Task ToList6()
        {
            var list=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="101").ToShardingListAsync();
            Assert.Empty(list);
        }
        [Fact]
        public async Task ToList7()
        {
            var list=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_101").ToShardingListAsync();
            Assert.Empty(list);
        }
        [Fact]
        public async Task FirstOrDefault1()
        {
            var sysUser=await _virtualDbContext.Set<SysUserMod>().OrderBy(o=>o.Id).ShardingFirstOrDefaultAsync();
            Assert.True(sysUser!=null&&sysUser.Id=="1");
        }
        [Fact]
        public async Task FirstOrDefault2()
        {
            var sysUser=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id=="1").ShardingFirstOrDefaultAsync();
            Assert.True(sysUser!=null&&sysUser.Id=="1");
        }
        [Fact]
        public async Task FirstOrDefault3()
        {
            var sysUser=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_2").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUser);
            Assert.Equal("2",sysUser.Id);
        }
        [Fact]
        public async Task FirstOrDefault4()
        {
            var sysUser=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Id!="1").ShardingFirstOrDefaultAsync();
            Assert.NotNull(sysUser);
            Assert.True(sysUser.Id!="1");
        }
        [Fact]
        public async Task FirstOrDefault5()
        {
            var sysUser=await _virtualDbContext.Set<SysUserMod>().Where(o=>o.Name=="name_101").ShardingFirstOrDefaultAsync();
            Assert.Null(sysUser);
        }
    }
}