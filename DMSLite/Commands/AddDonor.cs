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
        public Tuple<string, string> Execute()
        {
            return new Tuple<string, string>("Donors", "AddForm");
        }
    }
}