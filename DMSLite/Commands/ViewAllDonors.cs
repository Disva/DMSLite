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
    public class ViewAllDonors
    {
        public ActionResult Execute(Dictionary<string, object> parameters)
        {
            return new DonorsController().FetchDonor(parameters);
        }
    }
}