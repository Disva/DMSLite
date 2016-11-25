using System;
using System.Collections.Generic;
using ApiAiSDK;
using DMSLite.Models;
using Newtonsoft.Json;
using System.IO;

namespace DMSLite.Commands
{

    public class Dispatcher
    {
        private const string apiaikey = "9cc984ef80ef4502baa2de299ce11bbc"; //Client token used
        private const string CommandsLocation = "Commands.json";

        private static Dispatcher dispatcher;
        private ApiAi apiAi;

        public static Dispatcher getDispatcher()
        {
            if (dispatcher == null)
                dispatcher = new Dispatcher();
            return dispatcher;
        }

        private Dispatcher()
        {
            InitAPIAI();
        }

        private void InitAPIAI()
        {
            var config = new AIConfiguration(apiaikey, SupportedLanguage.English);
            apiAi = new ApiAi(config);
        }

        public ResponseModel Dispatch(string request)
        {
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            //Search commands file for appropriate command instructions
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Commands\", CommandsLocation);
            string thisPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = "";
            if (thisPath.Contains("DMSLite\\"))//for the DMS project
                path = Path.Combine(thisPath, @"Commands\", CommandsLocation);
            else//for the testing project
                path = Path.Combine("../../../DMSLite/", @"Commands\", CommandsLocation);
            StreamReader r = new StreamReader(path);

            string json = r.ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, Tuple<string, string>>>(json);

            ResponseModel responseModel = new ResponseModel()
            {
                Speech = response.Result.Fulfillment.Speech,
            };

            try
            {
                responseModel.Instructions = data[response.Result.Action];
                responseModel.Parameters = response.Result.Parameters;
            }
            catch
            {
                responseModel.Instructions = null;
                responseModel.Parameters = null;
            }

            return responseModel;
        }
    }

    internal class EmptyRequestException : Exception
    {
        public EmptyRequestException()
        {
        }
    }
}