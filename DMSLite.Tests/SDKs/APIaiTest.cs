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
        }

        [TestMethod]
        public void TestAddDonation()
        {
        }

        [TestMethod]
        public void TestModifyDonor()
        {
        }

        [TestMethod]
        public void TestViewDonor()
        {
            string name = "john smith";
            var response = apiAi.TextRequest("show me " + name);
            Assert.AreEqual(response.Result.Action, "ViewDonors");
            Assert.AreEqual(response.Result.Parameters["name"].ToString(), name, true);
        }

        [TestMethod]
        public void TestViewDonors()
        {

        }

        //Sends a randomly selected NL request to API.ai
        private AIResponse RandomTextInput(string[] inputs)
        {
            return apiAi.TextRequest(inputs[rand.Next(0, inputs.Length)]);
        }
    }
}
