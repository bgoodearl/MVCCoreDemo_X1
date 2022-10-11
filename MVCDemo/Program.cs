using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using NLog;
using NLog.Web;
using NLEL = NLog.Extensions.Logging;

namespace MVCDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitializeLogging();
            NLog.Logger logger = null;
            DateTime startTimeUtc = DateTime.UtcNow;

            try
            {
                if (logger == null) logger = NLog.LogManager.GetCurrentClassLogger();

                if (logger != null) logger.Info("MVCDemo Main start");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                if (logger == null) logger = NLog.LogManager.GetCurrentClassLogger();
                if (logger != null)
                    logger.Error(ex, "MVCDemo Main {0}: {1}", ex.GetType().Name, ex.Message);
                throw;
            }
            finally
            {
                TimeSpan elapsed = new TimeSpan(DateTime.UtcNow.Ticks - startTimeUtc.Ticks);
                if (logger == null) logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Info("MVCDemo Main end - elapsed: {0}", elapsed);

                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        #region Logging

        private static void InitializeLogging()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
#if DEBUG
                .AddJsonFile(GetUserJsonFilename(), optional: true, reloadOnChange: true)
#endif
                .Build();
            LogManager.Configuration = new NLEL.NLogLoggingConfiguration(config.GetSection("NLog"));
        }

        #endregion


        #region WebHost

        public static string GetUserJsonFilename()
        {
            return $"appsettings.user_{Environment.UserName.ToLower()}.json";
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile(GetUserJsonFilename(), true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection

        #endregion WebHost
    }
}
