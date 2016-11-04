using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DMSLite.Startup))]
namespace DMSLite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
