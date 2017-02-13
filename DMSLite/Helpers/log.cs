using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Threading;
using Microsoft.AspNet.Identity;

namespace DMSLite.Helpers
{
    static class Log
    {

        public static void writeLog(string logMessage, string logSymbol)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                WriteLog(logMessage, logSymbol, w);
            }

            using (StreamReader r = File.OpenText("log.txt"))
            {
                DumpLog(r);
            }
        }

        private static void WriteLog(string logMessage, string logSymbol, TextWriter w)
        {
            //logSymbol is 3 chars long; '<--' for user input, '-->' for output, ' ! ' for user reported bug, ' # ' for task
            w.WriteLine("{0} : user {1} : {2} : {3}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Thread.CurrentPrincipal.Identity.GetUserId(), logSymbol, logMessage);
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}