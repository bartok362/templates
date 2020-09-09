using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace StructuredLogging.Microsoft
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginScope(this ILogger logger, string key, object value)
        {
            return logger.BeginScope(new Dictionary<string, object> { { key, SerializeObject(value) } });
        }

        public static IDisposable BeginScope(this ILogger logger, params (string key, object value)[] parameters)
        {
            return logger.BeginScope(parameters.ToDictionary(x => x.key, x => (object)SerializeObject(x.value)));
        }

        private static string SerializeObject(object value)
        {
            if (value == null)
            {
                return "Object is null. Could not serialize";
            }
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception e)
            {
                return $"Could not serialize. Exception: {e.Message}";
            }
            //catch
            //{
            //    // we do not want to fail during logging
            //    return string.Empty;
            //}
        }
    }
}
