using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharding.Api.Domain.Entities;

namespace Sharding.Api.Domain.Maps
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 14 January 2021 15:37:33
* @Email: 326308290@qq.com
*/
    public class SysUserMap:IEntityTypeConfiguration<SysUser>
    {
        public void Configure(EntityTypeBuilder<SysUser> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).IsRequired().HasMaxLength(128);
            builder.Property(o => o.Name).HasMaxLength(128);
            builder.ToTable(nameof(SysUser));
        }
    }
}