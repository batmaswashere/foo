using System.Configuration;
using System.Data;
using log4net;
using log4net.Appender;


namespace OrderBookApp.Util
{
    internal class Logger
    {
        private static string baseFolderPath = Constants.BASE_FULL_FOLDER;
        private static ILog log = LogManager.GetLogger(typeof(OrderBookApp.Util.Logger));

        public static ILog createLog() {
            
            string today = DateTime.Now.ToString("yyyyMMdd");
            string filePath = baseFolderPath;
            filePath = filePath + @"log\";
            // Create log folder if not exist
            if (!Directory.Exists(filePath)) {
                System.IO.Directory.CreateDirectory(filePath);
            }
            string logFile = string.Format("log_{0}.txt", today);
            string logFileLocation = filePath + logFile;
            if (!File.Exists(logFileLocation))
            {
                using (StreamWriter w = File.AppendText(logFileLocation)) {
                    w.Close();
                }
            }

            ((FileAppender)LogManager.GetRepository().GetAppenders().Select(a => a).First()).File = logFileLocation;
            ((FileAppender)LogManager.GetRepository().GetAppenders().Select(a => a).First()).ActivateOptions(); 
            
            // Display the logging at Console (Remove)
            //log4net.Config.BasicConfigurator.Configure();  
            log = LogManager.GetLogger(typeof(OrderBookApp.Util.Logger));

            return log;
        }

        public static ILog GetLogger()
        {
            return log;
        }
    }
}
