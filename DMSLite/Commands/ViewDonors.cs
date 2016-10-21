using DMSLite.DataContexts;
using DMSLite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DMSLite.Commands
{
    public class ViewDonors : ICommand
    {
        public ActionResult Execute(ApiAiSDK.Model.Result result)
        {
             return new DonorsController().FetchDonor(result);
        }
    }
}