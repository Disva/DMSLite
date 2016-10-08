﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiAiSDK;
using System.Web.Mvc;

namespace DMSLite.Commands
{
    public class Dispatcher
    {
        private const string apiaikey = "9cc984ef80ef4502baa2de299ce11bbc"; //Client token used

        private static ApiAi apiAi;

        public Dispatcher()
        {
            InitAPIAI();

        }

        static void InitAPIAI()
        {
            var config = new AIConfiguration(apiaikey, SupportedLanguage.English);
            apiAi = new ApiAi(config);
        }

        public ActionResult Dispatch(string request)
        {
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            // Get Client's Project Name
            string project = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            // Taken from API.ai returned JSON object
            string action = response.Result.Action;

            //Join strings to create classLocation
            string classLocation = project + ".Commands." + action;
            try
            {
                //Find the class as a Type
                Type commandType = Type.GetType(classLocation);

                //Check if null, if null return message to the UI
                if (commandType == null) throw new NoCommandFoundException();
            
                //Cast and execute the command
                ICommand command = Activator.CreateInstance(commandType) as ICommand;

                //NOTE this returns a view
                //command.Execute(response.Result.Parameters);
                //return response.Result.Fulfillment.Speech;
                return command.Execute(response.Result.Parameters);
            }
            catch (NoCommandFoundException)
            {
                return new ShowErrorCommand("no command found").Execute(response.Result.Parameters);
            }
            catch (Exception e)
            {
                // Send Error message to the UI
                //return e.ToString();
                return new ShowErrorCommand(e.ToString()).Execute(response.Result.Parameters);
            }
        }
    }
    
    internal class NoCommandFoundException : Exception
    {
        public NoCommandFoundException()
        {
        }
    }
}