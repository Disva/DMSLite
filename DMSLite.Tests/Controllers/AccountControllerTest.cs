using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DMSLite.Tests.Mocks;
using DMSLite.Controllers;
using DMSLite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace DMSLite.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private FakeOrganizationDb db = new FakeOrganizationDb();
        AccountController ac;

        [TestInitialize]
        public void ControllerInit()
        {
            ac = new AccountController(db);
        }

        [TestMethod]
        public async Task TestRegisterAccount()
        {
            string password = "P@ssword123";

            ApplicationUser user = new ApplicationUser
            {
                TenantId = 0,
                UserName = "testing@testing.com",
                Email = "testing@testing.com",
            };
            
            var result = await ac.UserManager.CreateAsync(user, password);
            Assert.IsTrue(result.Succeeded);

            //await ac.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}
