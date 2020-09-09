using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StructuredLogging.Models;
using System;
using System.Collections.Generic;

namespace StructuredLogging.Serilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;

        public SampleController(ILogger<SampleController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Do NOT use string interpolation / string.Format() to construct the message
            // Use properties in the message template using {} and let the logger input the data (this preserves the message template)
            // Additional objects not used in the message are NOT added as properties in application insights
            // the @ instructs Serilog to deconstruct the object as json
            _logger.LogDebug("Accessed at {timestampUtc} by {@user}", DateTime.UtcNow, DataProvider.User);

            var data = DataProvider.ComplexObject;

            // Serilog honors scopes just as the default logger
            // it is important that the scope is of type Dictionary<string, object>, otherwise the properties are not correctly added in application insights
            // the '@' is important to instruct the logger to deconstruct the object, otherwise ToString() is used
            // Since this notation is not so handy, we can add an extension method as shown in LoggerExtensions
            // The message should be helpful even without the properties, because not all sinks/output targets can display those properly
            using (_logger.BeginScope(new Dictionary<string, object> { ["@data"] = data }))
            {
                _logger.LogInformation("Computed something with id {id}", data.Id);
            }

            return Ok();
        }

        [HttpGet]
        [Route("error")]
        public IActionResult GetError()
        {
            var value = new Random().Next(1, 1000);
            try
            {
                var zero = 0;
                var nan = value / zero;

                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to divide {value} by 0", value);
                throw;
            }
        }
    }
}
