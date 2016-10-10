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
        public ActionResult Execute(Dictionary<string, object> parameters)
        {
            if(parameters.ContainsKey("given-name"))
                return new DonorsController().FetchDonor(parameters["given-name"].ToString());
            return new DonorsController().FetchIndex();
        }
    }
}