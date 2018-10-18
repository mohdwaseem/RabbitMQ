using log4net;
using log4net.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;


namespace Receive
{
    public static class MessageLogger
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Method to Save Messages to database
        /// </summary>
        /// <param name="correlationId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int AddLogs(Guid correlationId, string message)
        {
            using (var connection = new SqlConnection("Data Source=W12928\\MSSQLSERVER1;Initial Catalog=RabbitMQLoggingDB;Integrated Security=True"))
            {
                connection.Open();
                var cmd = new SqlCommand("usp_tbl_MessageLogs_SaveLogs", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CorrelationId", correlationId);
                cmd.Parameters.AddWithValue("@MessageData", message);
                return cmd.ExecuteNonQuery();

            }
        }
        public static void AddMessageLogs()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            // thread properties...
            //log4net.LogicalThreadContext.Properties["CustomColumn"] = "Custom value";
            // ...or global properties
            log4net.GlobalContext.Properties["CustomColumn"] = "Custom value";
            log.Info("Message");
        }
    }
}
 
