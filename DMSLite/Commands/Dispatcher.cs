using System;
using System.Collections.Generic;
using ApiAiSDK;
using DMSLite.Models;
using Newtonsoft.Json;
using System.IO;
using System.Web.Configuration;

namespace DMSLite.Commands
{

    public class Dispatcher
    {
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
            var config = new AIConfiguration(WebConfigurationManager.AppSettings["APIAIKey"], SupportedLanguage.English);
            apiAi = new ApiAi(config);
        }

        public ResponseModel Dispatch(string request)
        {
            if (String.IsNullOrWhiteSpace(request))
            {
                ResponseModel emptyResponseModel = new ResponseModel()
                {
                    Speech = "Well, thats a whole lot of nothing. Try entering a command"
                    //link to commands page later?
                };
                return emptyResponseModel;
            }
            var response = apiAi.TextRequest(request);

            Console.WriteLine(response.Result.Fulfillment.Speech);

            //Search commands file for appropriate command instructions
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            StreamReader r = new StreamReader(path.Substring(6) + @"\Commands\" + CommandsLocation);
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