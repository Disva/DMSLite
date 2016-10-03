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

        public void Dispatch(string request)
        {
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            // Get Client's Project Name
            string project = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            // Taken from API.ai returned JSON object
            string action = response.Result.Action;

            //Join strings to create classLocation
            string classLocation = project + "." + action;

            //Find the class as a Type
            Type commandType = Type.GetType(classLocation);

            //Check if null, if null return message to the UI
            if (commandType == null) return;

            //Cast and execute the command
            iCommand command = Activator.CreateInstance(commandType) as iCommand;

            try
            {
                //NOTE this returns a view
                command.Execute(response.Result.Parameters);
            }
            catch (Exception e)
            {
                // Send Error message to the UI
                return;
            }
        }
    }
}