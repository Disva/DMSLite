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
        static private string path;

        public static void WriteLog(string logMessage, string logSymbol)
        {
            //Singleton pattern
            if (path == null)
                path = Path.GetTempPath().ToString() + String.Format("DMSLitelog-{0}.txt", DateTime.Now.ToString("dd-MM-yyyy HH-mm"));

            using (StreamWriter logWriter = File.AppendText(path))
            {
                /*
                logSymbol is 3 chars long; 
                '<--' user input
                '-->' output
                '->>' returned/recognized parameters
                ' ! ' user reported bug
                ' # ' task
                */
                logWriter.WriteLine("{0} : {1} : {2} : {3}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Thread.CurrentPrincipal.Identity.GetUserId(), logSymbol, logMessage);
            }
        }
    }
}