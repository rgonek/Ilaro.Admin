using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ilaro.Sample.Startup))]
namespace Ilaro.Sample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
