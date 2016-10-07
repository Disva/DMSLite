using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ApiAiSDK;

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

        public string Dispatch(string request)
        {
            if (String.IsNullOrWhiteSpace(request))
                return "Please enter a command";
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            // Get Client's Project Name
            string project = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            // Taken from API.ai returned JSON object
            string action = response.Result.Action;

            //Join strings to create classLocation
            string classLocation = project + CommandsLocation + action;

            //Find the class as a Type
            Type commandType = Type.GetType(classLocation);

            //Check if null, if null return message to the UI
            if (commandType == null) return "no command found";

            //Cast and execute the command
            ICommand command = Activator.CreateInstance(commandType) as ICommand;

            try
            {
                //NOTE this returns a view
                command.Execute(response.Result.Parameters);
                return response.Result.Fulfillment.Speech;
            }
            catch (Exception e)
            {
                // Send Error message to the UI
                return e.ToString();
            }
        }
    }
}