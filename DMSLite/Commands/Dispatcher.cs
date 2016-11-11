using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiAiSDK;
using System.Web.Mvc;
using DMSLite.Controllers;
using DMSLite.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

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

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Commands\", CommandsLocation);
            StreamReader r = new StreamReader(path);

            string json = r.ReadToEnd();
            var data = JsonConvert.DeserializeObject<Dictionary<string, Tuple<string, string>>>(json);

            try
            {
                //Check if null, if null return message to the UI
                if (data[response.Result.Action] == null) throw new Exception("No command found.");

                ResponseModel responseModel = new ResponseModel()
                {
                    Speech = response.Result.Fulfillment.Speech,
                    Instructions = data[response.Result.Action],
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
                    Parameters = null
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