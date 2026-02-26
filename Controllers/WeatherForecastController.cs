using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using brevet_tracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace brevet_tracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AppDbContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            try
            {
                await _dbContext.Database.EnsureCreatedAsync();

                var records = await _dbContext.WeatherForecasts
                    .OrderBy(x => x.Date)
                    .Take(5)
                    .ToListAsync();

                if (!records.Any())
                {
                    var rng = new Random();
                    records = Enumerable.Range(1, 5).Select(index => new WeatherForecastEntity
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    }).ToList();

                    await _dbContext.WeatherForecasts.AddRangeAsync(records);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(records.Select(record => new WeatherForecast
                {
                    Date = record.Date,
                    TemperatureC = record.TemperatureC,
                    Summary = record.Summary
                }).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read weather forecasts from MariaDB.");
                return StatusCode(503, "Database is unavailable. Check ConnectionStrings:MariaDb and MariaDb:ServerVersion in appsettings.");
            }
        }
    }
}
