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
        //Inner class for logging types
        public class LogType
        {
            private LogType(string value) { Value = value; }

            public string Value { get; set; }
            public override string ToString()
            {
                return Value;
            }

            public static LogType UserIn { get { return new LogType("<--"); } }
            public static LogType Reply { get { return new LogType("-->"); } }
            public static LogType ParamsFound { get { return new LogType("->>"); } }
            public static LogType ParamsSubmitted { get { return new LogType("<<-"); } }
            public static LogType Bug { get { return new LogType("!"); } }
            public static LogType Task { get { return new LogType("!"); } }
            public static LogType Test { get { return new LogType(" T "); } }
        }

        static private string path;

        public static string WriteLog(LogType logSymbol, string logMessage)
        {
            var fileLocation = String.Format("DMSLitelog-{0}.txt", DateTime.Now.ToString("dd-MM-yyyy HH-mm"));
            //Singleton pattern
            if (path == null)
                path = Path.GetTempPath().ToString() + fileLocation;

            using (StreamWriter logWriter = File.AppendText(path))
            {
                logWriter.WriteLine("{0} : {1} : {2} : {3}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Thread.CurrentPrincipal.Identity.GetUserId(), logSymbol, logMessage);
            }

            return fileLocation;
        }
    }
}