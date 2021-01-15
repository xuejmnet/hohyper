using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoHyper.DbContexts.VirtualDbContexts;
using HoHyper.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sharding.Api.Domain.Entities;

namespace Sharding.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IVirtualDbContext _virtualDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IVirtualDbContext virtualDbContext)
        {
            _logger = logger;
            _virtualDbContext = virtualDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            var queryable = _virtualDbContext.Set<SysUser>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Ok(await queryable.ToShardingListAsync());
            }
            else
            {
                return Ok(await queryable.Where(o=>o.Id==id).ToShardingListAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Gets(string ids)
        {
            var queryable = _virtualDbContext.Set<SysUser>();
            if (string.IsNullOrWhiteSpace(ids))
            {
                return Ok(await queryable.ToShardingListAsync());
            }
            else
            {
                var idlist = ids.Split(",");
                return Ok(await queryable.Where(o=>idlist.Contains(o.Id)).ToShardingListAsync());
            }
        }
        [HttpGet]
        public async Task<IActionResult> Insert(int i)
        {
            if (i > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    await _virtualDbContext.InsertAsync(new SysUser()
                    {
                        Id = Guid.NewGuid().ToString("n"),
                        Name = j.ToString(),
                        Age = j
                    });
                }

                await _virtualDbContext.SaveChangesAsync();
            }
            return Ok();
        }
    }
}