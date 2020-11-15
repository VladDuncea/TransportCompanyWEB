using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TransportCompany.Startup))]
namespace TransportCompany
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
