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
    public class AddDonor : ICommand
    {
        public ActionResult Execute(Dictionary<string, object> parameters, String speechLine)
        {
            return new DonorsController().AddForm(parameters);
        }
    }
}