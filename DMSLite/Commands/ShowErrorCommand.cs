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
        public Tuple<string, string> Execute()
        {
            return new Tuple<string, string>("Error", "ErrorMessage");
        }
    }
}