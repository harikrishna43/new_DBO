using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DBO.Startup))]
namespace DBO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
