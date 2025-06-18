using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBookApp
{
    class Constants
    {
        // Logger Path
        public const string BASE_FULL_FOLDER = @"D:\User Backup\Michael\Michael\Interview\IT\Standard Chartered Bank\2nd Interview\Programming Test OrderFeed\";
        
        // General
        public const string BASE_FOLDER = "baseFolder";
        public const string INPUT_STREAM_FILE_1 = "input1.stream";
        public const string INPUT_STREAM_FILE_2 = "input2.stream";
        public const string OUTPUT_FILE_1 = "output1.log";
        public const string OUTPUT_FILE_2 = "output2.log";

        // Order Type
        public const string ORDER_TYPE_ADDED = "A";
        public const string ORDER_TYPE_UPDATED = "U";
        public const string ORDER_TYPE_DELETED = "D";
        public const string ORDER_TYPE_EXECUTED = "E";

        // Order Side
        public const string SIDE_BUY_ORDER = "B";
        public const string SIDE_SELL_ORDER = "S";
    }
}
