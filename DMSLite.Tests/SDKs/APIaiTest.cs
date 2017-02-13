using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApiAiSDK;
using ApiAiSDK.Model;
using System.Configuration;

namespace DMSLite.Tests.SDKs
{


    [TestClass]
    public class APIaiTest
    {
        /**
         * One must make a test method per Intent
         * It would also be beneficial to include cases where API.ai *should* fail
         * Test for returned Action and Parameters.
         * Include varied input to test for robustness
         **/

        private static ApiAi apiAi;
        private Random rand = new Random();

        [TestInitialize]
        public void APIAIInit()
        {
            string key = "";
#if DEBUG
            key = ConfigurationManager.AppSettings["APIAIKey-Debug"];
#else
            key = ConfigurationManager.AppSettings["APIAIKey-Release"];
#endif
            apiAi = new ApiAi(new AIConfiguration(key, SupportedLanguage.English));
        }

        /**
        [TestMethod]
        public void APITestContextConversation()
        {
            //Since it is missing a parameter, it is not a complete action
            var response = apiAi.TextRequest("add new batch");
            Assert.AreEqual("AddBatch", response.Result.Action);
            Assert.IsTrue(response.Result.ActionIncomplete);
            Assert.IsFalse(response.Result.Contexts.Length == 0);

            response = apiAi.TextRequest("my batch title");
            Assert.IsFalse(response.Result.ActionIncomplete);
        }
        **/


        [TestMethod]
        public void APITestAddBatch()
        {
            return;
            string batchTitle = "Birthday Party";

            string[] inputs =
            {
                "add a batch {0}",
                "add new batch {0}"
            };

            var response = apiAi.TextRequest("add new batch");
            Assert.AreEqual(response.Result.Action, "AddBatch");
            Assert.IsTrue(String.IsNullOrWhiteSpace(response.Result.Parameters["title"].ToString()));

            var nameResponse = RandomTextInput(inputs, batchTitle);
            Assert.AreEqual(response.Result.Action, "AddBatch");
            Assert.AreEqual(nameResponse.Result.Parameters["title"].ToString(), batchTitle);
        }

        [TestMethod]
        public void APITestAddDonor()
        {
            string name = "john smith";
            string email = "steve@stevemail.com";
            string phoneNumber = "555-555-5555";

            string[] inputs =
            {
                "add {0}",
                "create {0}"
            };

            //With No params
            var blankResponse = apiAi.TextRequest("create new donor");
            Assert.AreEqual(blankResponse.Result.Action,  "AddDonor");
            Assert.IsTrue(String.IsNullOrWhiteSpace(blankResponse.Result.Parameters["name"].ToString()));

            //By name
            var nameResponse = RandomTextInput(inputs, name);
            Assert.AreEqual(nameResponse.Result.Action, "AddDonor");
            Assert.AreEqual(nameResponse.Result.Parameters["name"].ToString().ToLower(), name.ToLower());

            //By phone number
            var phoneResponse = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(phoneResponse.Result.Action, "AddDonor");
            Assert.AreEqual(phoneResponse.Result.Parameters["phone-number"].ToString().ToLower(), phoneNumber.ToLower().Replace("-", ""));

            //By email
            var emailResponse = RandomTextInput(inputs, email);
            Assert.AreEqual(emailResponse.Result.Action, "AddDonor");
            Assert.AreEqual(emailResponse.Result.Parameters["email-address"].ToString(), email);

        }

        [TestMethod]
        public void APITestAddDonation()
        {
            string[] inputs =
            {
                "add a donation {0}",
            };

            var response = apiAi.TextRequest("add new donation");
            Assert.AreEqual(response.Result.Action, "AddDonation");
        }

        [TestMethod]
        public void APITestViewDonor()
        {
            string name = "john smith";
            string email = "steve@stevemail.com";
            string phoneNumber = "555-555-5555";

            string[] inputs =
            {
                "show me {0}",
                "View {0}"
            };

            //By name
            var response = RandomTextInput(inputs, name);
            Assert.AreEqual(response.Result.Action, "ViewDonors");
            Assert.AreEqual(response.Result.Parameters["name"].ToString(), name, true);

            //By email
            response = RandomTextInput(inputs, email);
            Assert.AreEqual(response.Result.Action, "ViewDonors");
            Assert.AreEqual(response.Result.Parameters["email-address"].ToString(), email, true);

            //By phone number
            response = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(response.Result.Action, "ViewDonors");
            Assert.AreEqual(response.Result.Parameters["phone-number"].ToString(), phoneNumber.Replace("-",""), true);

        }

        [TestMethod]
        public void APITestModifyDonor()
        {
            string name = "john smith";
            string email = "steve@stevemail.com";
            string phoneNumber = "555-555-5555";

            string[] inputs =
            {
                "edit {0}"
            };

            //By email
            var emailResponse = RandomTextInput(inputs, email);
            Assert.AreEqual(emailResponse.Result.Action, "ModifyDonor");
            Assert.AreEqual(emailResponse.Result.Parameters["email-address"].ToString(), email, true);

            //By name
            var nameResponse = RandomTextInput(inputs, name);
            Assert.AreEqual(nameResponse.Result.Action, "ModifyDonor");
            Assert.AreEqual(nameResponse.Result.Parameters["name"].ToString(), name, true);

            //By phone number
            var phoneResponse = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(phoneResponse.Result.Action, "ModifyDonor");
            Assert.AreEqual(phoneResponse.Result.Parameters["phone-number"].ToString(), phoneNumber.Replace("-",""), true);
        }

        [TestMethod]
        public void APITestViewDonors()
        {
            
            string[] inputs =
            {
                "show donors",
                "View donors"
            };

            //test Action
            var response = RandomTextInput(inputs);
            Assert.AreEqual(response.Result.Action, "ViewAllDonors");
        }

        [TestMethod]
        public void APITestViewListOfBatches()
        {
            string open = "open", closed = "closed";

            string[] inputs =
            {
                "view {0} batches",
                "show all {0} batches"
            };

            var response = RandomTextInput(inputs, "");
            Assert.AreEqual(response.Result.Action, "ViewBatches");
           
            response = RandomTextInput(inputs, open);
            Assert.AreEqual(response.Result.Parameters["type"].ToString(), open, true);

            response = RandomTextInput(inputs, closed);
            Assert.AreEqual(response.Result.Parameters["type"].ToString(), closed, true);

        }

        [TestMethod]
        public void APITestViewSingleBatch()
        {
            string closed = "closed";

            string title = "new batch title";

            string[] inputs =
            {
                "view {0} batch {1}",
                "show {0} batches {1}"
            };

            var response = RandomTextInput(inputs, closed, title);
            Assert.AreEqual(response.Result.Action, "ViewBatches");

            Assert.AreEqual(response.Result.Parameters["type"].ToString(), closed, true);

            Assert.AreEqual(response.Result.Parameters["title"].ToString(), title, true);

        }

        [TestMethod]
        public void APITestFilterBatches()
        {
            return; 

            string april17th = "2017-04-17";

            string[] inputs =
            {
                "show {0} batches {1} {2}"
            };

            var response = RandomTextInput(inputs, "closed", "before", "april 17th");
            Assert.AreEqual(response.Result.Action, "ViewBatches");

            Assert.AreEqual(response.Result.Parameters["date"].ToString(), april17th, true);

            Assert.AreEqual(response.Result.Parameters["datetype"].ToString(), "before", true);

            Assert.AreEqual(response.Result.Parameters["type"].ToString(), "closed", true);
        }

        [TestMethod]
        public void APITestHelp()
        {
            string[] inputs =
            {
                "help",
                "help me",
                "sos"
            };

            var response = RandomTextInput(inputs);
            Assert.AreEqual(response.Result.Action, "Help");
        }

        [TestMethod]
        public void APITestViewDonations()
        {
            string donor = "from James Bond", value = "over 100$", account = "from the Phoenix Convention Account";

            string[] inputs =
            {
                "view donations {0}",
                "show all donations {0}"
            };

            var response = RandomTextInput(inputs, "");
            Assert.AreEqual(response.Result.Action, "ViewDonations");

            response = RandomTextInput(inputs, donor);
            Assert.AreEqual(response.Result.Parameters["donor-name"].ToString(), "James Bond", true);

            response = RandomTextInput(inputs, value);
            Assert.AreEqual(response.Result.Parameters["value"].ToString(), "100", true);
            Assert.AreEqual(response.Result.Parameters["value-comparator"].ToString(), ">", true);

            response = RandomTextInput(inputs, account);
            Assert.AreEqual(response.Result.Parameters["account-name"].ToString(), "Phoenix Convention", true);
        }

        //Sends a randomly selected NL request to API.ai
        private AIResponse RandomTextInput(string[] inputs, params string[] values)
        {
            return apiAi.TextRequest(String.Format(inputs[rand.Next(0, inputs.Length)], values));
        }
    }
}
