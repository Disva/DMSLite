using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using Microsoft.AspNet.Identity;

namespace DMSLite.Helpers
{
    public static class Log
    {
        public static void WriteLog(string logMessage, string logSymbol)
        {
            using (StreamWriter w = File.AppendText(Path.GetTempPath().ToString()+"DMSLitelog.txt"))
            {
                WriteLog(logMessage, logSymbol, w);
            }
        }

        private static void WriteLog(string logMessage, string logSymbol, TextWriter w)
        {
            /*
            logSymbol is 3 chars long; 
            '<--' for user input
            '-->' for output
            ' ! ' for user reported bug
            ' # ' for task
            */
            w.WriteLine("{0} : {1} : {2} : {3}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Thread.CurrentPrincipal.Identity.GetUserId(), logSymbol, logMessage);
        }
    }
}