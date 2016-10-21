using DMSLite.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DMSLite.Commands
{
    public class ShowErrorCommand : ICommand
    {
        public string errorMessage;

        public ShowErrorCommand(string error)
        {
            errorMessage = error;
        }

        public ActionResult Execute(ApiAiSDK.Model.Result result)
        {
            return new ErrorController().ErrorMessage(errorMessage);
        }
    }
}