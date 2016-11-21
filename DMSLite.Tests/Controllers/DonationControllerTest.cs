using DMSLite.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DMSLite.Entities;
using DMSLite.DataContexts;
using DMSLite.Models;

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class DonationControllerTest
    {
        private OrganizationDb db = new OrganizationDb();



        ////VALIDITY TESTS
        //[TestMethod]
        ////Tests that viewing all donors returns the list of all donors.
        //public void TestViewDonors()
        //{
        //    //works under the assumption that no donors with with null values for names, phonenumber, or email exist
        //    //if this test ever fails, remove your invalidly inserted donors from the db
        //    DonorsController ds = new DonorsController();
        //    PartialViewResult pvr = (PartialViewResult)ds.ViewAllDonors();
        //    List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
        //    List<Donor> allDonors = db.Donors.ToList();
        //    Assert.AreEqual(allDonors.Count(), returnedModel.Count());
        //    Console.WriteLine(allDonors.Count());
        //    Console.WriteLine(returnedModel.Count());
        //    for (int i = 0; i < allDonors.Count(); i++)
        //    {
        //        Assert.IsTrue(allDonors[i].FirstName == returnedModel[i].FirstName);
        //    }
        //}

    }
}
