using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;


namespace Receive
{
    public static class MessageLogger
    {

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            return configuration;
        }
        /// <summary>
        /// Method to Log Messages to database
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int SaveMessagesToDatabase(Guid correlationId, string message, string result)
        {
            using (var connection = new SqlConnection(GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var cmd = new SqlCommand("usp_tbl_MessageLogs_SaveLogs", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CorrelationId", correlationId);
                cmd.Parameters.AddWithValue("@MessageData", message);
                cmd.Parameters.AddWithValue("@Result", result);
                return cmd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Method to Update Log based on correlationId
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int UpdateMessageLog(string correlationId, string result)
        {

            using (var connection = new SqlConnection(GetConfiguration().GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var cmd = new SqlCommand("usp_tbl_Logs_UpdateLog", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CorrelationId", correlationId);
                cmd.Parameters.AddWithValue("@Result", result);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Method to Save Logs Using NLog
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        public static void Log(Guid correlationId, string message, string result)
        {
            var servicesProvider = BuildDi();
            Logger logger = LogManager.GetCurrentClassLogger();
            LogEventInfo theEvent = new LogEventInfo(NLog.LogLevel.Debug, "Inbound Queue", message);
            theEvent.Properties["CorrelationId"] = correlationId;
            theEvent.Properties["Result"] = result;
            logger.Log(theEvent);
            LogManager.Shutdown();

        }
        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug));
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("NLog.config");
            return serviceProvider;
        }
    }
}

