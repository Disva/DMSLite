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
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            //Search commands file for appropriate command instructions
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
                Speech = response.Result.Fulfillment.Speech
                //other properties assumed null
            };

            if (!response.Result.ActionIncomplete && data.ContainsKey(response.Result.Action))
            {
                responseModel.Instructions = data[response.Result.Action];
                responseModel.Parameters = response.Result.Parameters;
            }

            return responseModel;
        }
    }
}