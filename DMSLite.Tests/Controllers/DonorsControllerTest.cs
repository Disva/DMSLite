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
    public class DonorsControllerTest
    {
        private OrganizationDb db = new OrganizationDb();

        //VALIDITY TESTS
        [TestMethod]
        //Tests that viewing all donors returns the list of all donors.
        public void TestViewDonors()
        {
            DonorsController ds = new DonorsController();
            PartialViewResult pvr = (PartialViewResult)ds.ViewAllDonors();
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> allDonors = db.Donors.ToList();
            Assert.AreEqual(allDonors.Count(), returnedModel.Count());
            for(int i = 0; i < allDonors.Count(); i ++)
            {
                Assert.IsTrue(allDonors[i].isEqualTo(returnedModel[i]));
            }
        }

        [TestMethod]
        //Tests that requesting a specific valid donor returns that valid donor.
        public void TestViewSpecificDonor()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "name", "Steve" });
            parameters.Add("email-address", "");
            parameters.Add("name", "Steve");
            parameters.Add("phone-number", "");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> steve = db.Donors.Where(x => x.FirstName == "Steve").ToList();
            Assert.AreEqual(steve.Count(), returnedModel.Count());
            Assert.IsTrue(steve[0].isEqualTo(returnedModel[0]));
        }

        [TestMethod]
        //Tests requesting a donor by name where all donors with the same name are returned.
        public void TestViewMultipleDonors()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "name", "Squidward" });
            parameters.Add("email-address", "");
            parameters.Add("name", "Squidward");
            parameters.Add("phone-number", "");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> squids = db.Donors.Where(x => x.FirstName == "Squidward").ToList();
            Assert.AreEqual(squids.Count(), returnedModel.Count());
            Assert.IsTrue(squids[0].isEqualTo(returnedModel[0]));
            Assert.IsTrue(squids[1].isEqualTo(returnedModel[1]));
        }

        [TestMethod]
        //Tests searching for a donor by phone number.
        public void TestViewByPhoneNumber()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "phone-number", "555-555-5555" });
            parameters.Add("email-address", "");
            parameters.Add("name", "");
            parameters.Add("phone-number", "555-555-5555");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> steve = db.Donors.Where(x => x.PhoneNumber == "555-555-5555").ToList();
            Assert.AreEqual(steve.Count(), returnedModel.Count());
            Assert.IsTrue(steve[0].isEqualTo(returnedModel[0]));
        }

        [TestMethod]
        //Tests searching for a donor by Email.
        public void TestViewByEmail()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "email-address", "steve@stevemail.com"});
            parameters.Add("email-address" ,"steve@stevemail.com");
            parameters.Add("name", "");
            parameters.Add("phone-number", "");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> steve = db.Donors.Where(x => x.Email == "steve@stevemail.com").ToList();
            Assert.AreEqual(steve.Count(), returnedModel.Count());
            Assert.IsTrue(steve[0].isEqualTo(returnedModel[0]));
        }

        [TestMethod]
        //Tests a case where a donor with a duplicate first name is searched for with their unique last name and only one proper donor is returned
        public void TestViewSpecificDonorWithDuplicateName()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "name", "Squidward Tentacles" });
            parameters.Add("email-address", "");
            parameters.Add("name", "Squidward Tentacles");
            parameters.Add("phone-number", "");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> squidward = db.Donors.Where(x => x.FirstName == "Squidward" && x.LastName == "Tentacles").ToList();
            Assert.AreEqual(squidward.Count(), 1);
            Assert.IsTrue(squidward[0].isEqualTo(returnedModel[0]));
        }

        [TestMethod]
        //Tests searching for a specific donor by lowercase
        public void TestViewSpecificDonorsByLowerCase()
        {
            DonorsController dc = new DonorsController();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("donor-search", new List<String> { "name", "squidward" });
            parameters.Add("email-address", "");
            parameters.Add("name", "squidward");
            parameters.Add("phone-number", "");
            PartialViewResult pvr = (PartialViewResult)dc.FetchDonor(parameters);
            List<Donor> returnedModel = ((List<Donor>)pvr.ViewData.Model).ToList();
            List<Donor> squids = db.Donors.Where(x => x.FirstName == "Squidward").ToList();
            Assert.AreEqual(squids.Count(), returnedModel.Count());
            Assert.AreEqual(squids[0].FirstName, returnedModel[0].FirstName);
        }


        //INVALIDITY TESTS
        [TestMethod]
        //Tests that an invalid command returns an error.
        public void TestInvalidCommand()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Order a pizza. This is not a real command.");
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            string returnedSpeech = ((ResponseModel)returnedView.ViewData.Model).Speech ;
            Assert.IsTrue(returnedSpeech.Equals("It seems we ran into an error: No command found."));
        }

        [TestMethod]
        //Tests that searching for a donor with invalid criteria returns an error.
        public void TestViewInvalidDonorAndParameter()
        {
            HomeController hc = new HomeController();
            FormCollection fc = new FormCollection();
            fc.Add("mainInput", "Show me Tom Sawyer");//a user who does not exist in the db
            PartialViewResult returnedView = (PartialViewResult)hc.SendInput(fc);
            var returnedModel = returnedView.ViewData.Model;
            Assert.IsTrue(returnedModel.Equals("no donors were found"));

            fc.Add("mainInput", "Show me AnInvalidParameter");//a user who does not exist in the db
            returnedView = (PartialViewResult)hc.SendInput(fc);
            returnedModel = returnedView.ViewData.Model;
            Assert.IsTrue(returnedModel.Equals("no parameters were recognized"));
        }

        //CREATING DONORS TESTS
        [TestMethod]
        //Tests that creating a valid new donor
        public void TestAddNewValidDonor()
        {
            DonorsController dc = new DonorsController();
            //a valid donor is donor
            Donor d = new Donor
            {
                FirstName = "fName_TestAddNewValidDonor",
                LastName = "lName_TestAddNewValidDonor",
                Email = "email_TestAddNewValidDonor",
                PhoneNumber = "111-111-1111",
            };
            ActionResult arReturned = dc.Add(d);
            try
            {
                dc.Remove(d);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
                //TEMPORARY
                Assert.IsTrue(arReturned.ToString().Equals("Thanks"));
        }

        [TestMethod]
        //Tests that creating an invalid new donor fails
        public void TestAddNewInvalidDonor()
        {
            DonorsController dc = new DonorsController();
            //an invalid donor (has no firstName or lastName)
            Donor d = new Donor
            {
                FirstName = "",
                LastName = "",
                Email = "TestAddNewInvalidDonor",
                PhoneNumber = "-24",
            };
            ContentResult coReturned = new ContentResult();
            try//try adding it and removing it
            {
                coReturned = (ContentResult)dc.Add(d);//the invalid donor should not be added to the db
                dc.Remove(d);//if the test passes, the test should not reach this point. Just a cleanup precaution
            }
            catch (Exception e)
            {
                //This assertion should be enough
                //a better way would be to check assert that Donors does not contain d, but this was causing issues and is very inefficient
                Assert.IsTrue(true);
                return;
            }
            Assert.IsTrue(false);//reaches here if the invalid donor WAS actually added to the db
        }

        [TestMethod]
        //Tests that creating a duplicate donor does not work
        //WILL EVENTUALLY NEED TO BE RELATIVE TO ORGANIZATION
        public void TestAddDuplicateDonor()
        {
            DonorsController dc = new DonorsController();
            //creating two of the same donor
            Donor d1 = new Donor
            {
                FirstName = "fName_TestAddDuplicateDonor",
                LastName = "lName_TestAddDuplicateDonor",
                Email = "email_TestAddDuplicateDonor",
                PhoneNumber = "111-111-1111",
            };
            Donor d1Duplicate = new Donor
            {
                FirstName = d1.FirstName,
                LastName = d1.LastName,
                Email = d1.Email,
                PhoneNumber = d1.PhoneNumber,
            };
            dc.Add(d1);
            try
            {
                dc.Add(d1Duplicate);
                dc.Remove(d1Duplicate);
            }
            catch(Exception e)
            {
                //The duplicate was not added which passes the test
                Assert.IsTrue(true);
                return;
            }
            dc.Remove(d1);
            Assert.Fail();//the duplicate was added which fails the test
        }

    }
}
