using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace DMSLite.Commands
{
    public class ViewDonors : ICommand
    {
        public View Execute(Dictionary<string, object> parameters)
        {
            //SELECT * FROM Donors WHERE DonorOrganization_Id = _MY_ID_
            //put it in an object
            //return it to the controller
            throw new NotImplementedException();
        }
    }
}