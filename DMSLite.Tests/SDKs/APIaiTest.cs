using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ApiAiSDK;
using ApiAiSDK.Model;

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

        private const string apiaikey = "9cc984ef80ef4502baa2de299ce11bbc"; //Client token used
        private static ApiAi apiAi = new ApiAi(new AIConfiguration(apiaikey, SupportedLanguage.English));
        private Random rand = new Random();

        [TestMethod]
        public void TestContextConversation()
        {
            //Since it is missing a parameter, it is not a complete action
            var response = apiAi.TextRequest("add new batch");
            Assert.AreEqual("AddBatch", response.Result.Action);
            Assert.IsTrue(response.Result.ActionIncomplete);
            Assert.IsFalse(response.Result.Contexts.Length == 0);

            response = apiAi.TextRequest("my batch title");
            Assert.IsFalse(response.Result.ActionIncomplete);
        }

        [TestMethod]
        public void TestAddBatch()
        {
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
        public void TestAddDonor()
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
            Assert.AreEqual(nameResponse.Result.Parameters["name"].ToString(), name, true);

            //By email
            var emailResponse = RandomTextInput(inputs, email);
            Assert.AreEqual(emailResponse.Result.Action, "AddDonor");
            Assert.AreEqual(emailResponse.Result.Parameters["email-address"].ToString(), email, true);

            //By phone number
            var phoneResponse = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(phoneResponse.Result.Action, "AddDonor");
            Assert.AreEqual(phoneResponse.Result.Parameters["phone-number"].ToString(), phoneNumber, true);

        }

        [TestMethod]
        public void TestAddDonation()
        {
            string[] inputs =
            {
                "add a donation {0}",
            };

            var response = apiAi.TextRequest("add new donation");
            Assert.AreEqual(response.Result.Action, "AddDonation");
            Assert.IsTrue(response.Result.Parameters.Count == 0);
            /*NOTE
                At this point, the parameter list is empty
                This will change in the future, and donations will be added similarly to donors
            */
        }

        [TestMethod]
        public void TestViewDonor()
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
            Assert.AreEqual(response.Result.Parameters["phone-number"].ToString(), phoneNumber, true);

        }

        [TestMethod]
        public void TestModifyDonor()
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
            Assert.AreEqual(phoneResponse.Result.Parameters["phone-number"].ToString(), phoneNumber, true);
        }

        [TestMethod]
        public void TestViewDonors()
        {
            string[] inputs =
            {
                "show donors",
                "View donors"
            };

            //By name
            var response = RandomTextInput(inputs);
            Assert.AreEqual(response.Result.Action, "ViewAllDonors");
        }

        //Sends a randomly selected NL request to API.ai
        private AIResponse RandomTextInput(string[] inputs, params string[] values)
        {
            return apiAi.TextRequest(String.Format(inputs[rand.Next(0, inputs.Length)], values));
        }
    }
}
