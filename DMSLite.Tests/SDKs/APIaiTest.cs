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
        public void TestAddBatch()
        {
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
            var response = apiAi.TextRequest("create new donor");
            Assert.AreEqual(response.Result.Action,  "AddDonor");
            Assert.IsTrue(String.IsNullOrWhiteSpace(response.Result.Parameters["name"].ToString()));

            //By name
            response = RandomTextInput(inputs, name);
            Assert.AreEqual(response.Result.Action, "AddDonor");
            Assert.AreEqual(response.Result.Parameters["name"].ToString(), name, true);

            //By email
            response = RandomTextInput(inputs, email);
            Assert.AreEqual(response.Result.Action, "AddDonor");
            Assert.AreEqual(response.Result.Parameters["email-address"].ToString(), email, true);

            //By phone number
            response = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(response.Result.Action, "AddDonor");
            Assert.AreEqual(response.Result.Parameters["phone-number"].ToString(), phoneNumber, true);

        }

        [TestMethod]
        public void TestAddDonation()
        {
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

            //By name
            var response = RandomTextInput(inputs, name);
            Assert.AreEqual(response.Result.Action, "ModifyDonor");
            Assert.AreEqual(response.Result.Parameters["name"].ToString(), name, true);

            //By email
            response = RandomTextInput(inputs, email);
            Assert.AreEqual(response.Result.Action, "ModifyDonor");
            Assert.AreEqual(response.Result.Parameters["email-address"].ToString(), email, true);

            //By phone number
            response = RandomTextInput(inputs, phoneNumber);
            Assert.AreEqual(response.Result.Action, "ModifyDonor");
            Assert.AreEqual(response.Result.Parameters["phone-number"].ToString(), phoneNumber, true);
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
