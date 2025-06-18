using System;
using System.Configuration;
using log4net;
using log4net.Repository.Hierarchy;
using OrderBookApp.Controller;
using OrderBookApp.TradeEngine;
using OrderBookApp.Util;
using OrderBookApp.View;
using Logger = OrderBookApp.Util.Logger;

namespace OrderBookApp
{
    internal class Program
    {
        private ILog logger;

        public void LoadingConfiguration()
        {
            logger = Logger.createLog();
        }

        public ILog GetLogger()
        {
            return logger;
        }

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.LoadingConfiguration();
            ILog log = prog.GetLogger();

            //string readline = Console.ReadLine();
            //log.Info(string.Format("readline: {0}", readline));

            // Step 0. Read the top N Level of price depth snapshot
            log.Info("Step 0. Read the top N Level of price depth snapshot");
            int NLevel = 3;
            bool isParsed = false;
            log.Info(string.Format("Length of arguments: {0}", args.Length));
            //Console.WriteLine(string.Format("Length of arguments: {0}", args.Length));
            if (args.Length == 1)
            {
                isParsed = Int32.TryParse(args[0], out NLevel);
                log.Info(string.Format("NLevel: {0}", NLevel.ToString()));
                //Console.WriteLine(string.Format("NLevel: {0}", NLevel.ToString()));
            }

            // Step 1. Read from config
            log.Info("Step 1. Read from config");
            string baseFilePath = ConfigurationManager.AppSettings[Constants.BASE_FOLDER];
            // Binary encoded and little endian
            //string inputFileName = Constants.INPUT_STREAM_FILE_1;
            string inputFileName = Constants.INPUT_STREAM_FILE_2;
            string inputFileFullPath = String.Concat(baseFilePath, inputFileName);
            //string outputFile = Constants.OUTPUT_FILE_1;
            string outputFile = Constants.OUTPUT_FILE_2;
            string outputFilePath = baseFilePath + @"output\";
            if (!Directory.Exists(outputFilePath))
            {
                System.IO.Directory.CreateDirectory(outputFilePath);
            }
            string outputFileFullPath = String.Concat(outputFilePath, outputFile);

            // Step 2a. Reading the stream input file
            // Step 2b. Parse the message, header and order
            log.Info("Step 2a. Reading the stream input file");
            FileHandler handler = new FileHandler();
            //string msg = handler.ReadFile(inputFileFullPath);
            List<Order.Order> orderList = handler.ReadBinaryFile(inputFileFullPath);
            //log.Info(String.Format("Content =  {0}", msg));
            //Console.WriteLine(String.Format("Content =  {0}", msg));

            // Step 3. Proceed the orders
            log.Info("Step 3. Proceed the orders and Print the price depth, Output log file");
            TradeOrderEngine engine = new TradeOrderEngine();
            //List<PriceDepthNormalView> viewList = engine.Execute(orderList);
            engine.Execute(orderList, outputFileFullPath, NLevel);

            // Step 4. Display in the price depth view, Output log file
            // log.Info("Step 4. Display in the price depth view, Output log file");
            //handler.WriteOutputLogFile(outputFileFullPath, viewList, NLevel);
            log.Info("Finished!");
        }
    }
}
