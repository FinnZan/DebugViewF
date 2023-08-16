using System;
using System.Collections.Generic;

namespace FinnZan.Utilities
{
    public class CommonTools
    {
        private static bool _logEnabled = false;
        private static string _appName = "FinnZan";

        private static List<string> _outQueue = new List<string>();

        public static string InitializeDebugger(string appName)
        {
            _appName = appName;

            LoggerCore.Start(_appName, 2);
            _logEnabled = true;

            return "";
        }

        public static void Log(string log, int levelShift = 0)
        {
            if (_logEnabled)
            {
                LoggerCore.Log(log, levelShift);
            }
        }

        public static void HandleException(Exception ex)
        {
            if (_logEnabled)
            {
                LoggerCore.HandleException(ex);
            }
        }

        public static void Watch(string key, string value) 
        {
            LoggerCore.Watch(key, value);
        }
    }
}
