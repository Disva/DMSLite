using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DMSLite.Commands
{
    public interface ICommand
    {
       ActionResult Execute(Dictionary<String, Object> parameters);
    }
}
