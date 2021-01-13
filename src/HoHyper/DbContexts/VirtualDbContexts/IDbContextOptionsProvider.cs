using System;
using Microsoft.EntityFrameworkCore;

namespace HoHyper.DbContexts.VirtualDbContexts
{
/*
* @Author: xjm
* @Description:
* @Date: Thursday, 24 December 2020 10:32:07
* @Email: 326308290@qq.com
*/
    public interface IDbContextOptionsProvider:IDisposable
    {
        DbContextOptions GetDbContextOptions();
    }
}