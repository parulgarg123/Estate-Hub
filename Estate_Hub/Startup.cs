using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Proptiwise.Startup))]
namespace Proptiwise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
