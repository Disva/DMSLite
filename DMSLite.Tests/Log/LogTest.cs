using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace DMSLite.Tests.Log
{
    [TestClass]
    public class LogTest
    {
        [TestMethod]
        public void TestWriteToLog()
        {
            //generate a random number between 111111 and 999999
            Random rnd = new Random();
            int uniqueID = rnd.Next(111111, 999999);

            //log it
            Helpers.Log.WriteLog("Testing with uniqueID "+ uniqueID, " T ");

            //read the last line in the log and extract the uniqueID
            var lastLine = File.ReadLines("log.txt").Last();
            var retrievedId = lastLine.Substring(lastLine.Length - 6);

            //compare the random number to that found in the log file
            Assert.IsTrue(String.Equals(uniqueID.ToString(), retrievedId));
        }
    }
}
