using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiAiSDK;
using System.Web.Mvc;
using DMSLite.Controllers;
using DMSLite.Models;

namespace DMSLite.Commands
{ 

    public class Dispatcher
    {
        private const string apiaikey = "9cc984ef80ef4502baa2de299ce11bbc"; //Client token used
        private const string CommandsLocation = ".Commands.";

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

        public ResponseModel Dispatch(string request)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(request)) throw new EmptyRequestException();
            }
            catch (EmptyRequestException)
            {
                ResponseModel responseModel = new ResponseModel()
                {
                    Speech = "Please enter a command.",
                    Instructions = null,
                    Parameters = null
                };

                return responseModel;
            }

            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            // Get Client's Project Name
            string project = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            // Taken from API.ai returned JSON object
            string action = response.Result.Action;

            //Join strings to create classLocation
            string classLocation = project + ".Commands." + action;
            
            //Find the class as a Type
            Type commandType = Type.GetType(classLocation);
            try
            {
                //Check if null, if null return message to the UI
                if (commandType == null) throw new Exception("no command found");
            
                //Cast and execute the command
                ICommand command = Activator.CreateInstance(commandType) as ICommand;

                //NOTE this returns a view
                //command.Execute(response.Result.Parameters);
                //return response.Result.Fulfillment.Speech;
                ResponseModel responseModel = new ResponseModel()
                {
                    Speech = response.Result.Fulfillment.Speech,
                    Instructions = command.Execute(),
                    Parameters = response.Result.Parameters
                };

                return responseModel;
            }
            catch (Exception e)
            {
                // Send Error message to the UI
                //return e.ToString();
                ResponseModel responseModel = new ResponseModel()
                {
                    Speech = "It seems we ran into an error: " + e.Message,
                    Instructions = null,
                    Parameters =  null
                };

                return responseModel;
            }
        }
    }

    internal class EmptyRequestException : Exception
    {
        public EmptyRequestException()
        {
        }
    }
}