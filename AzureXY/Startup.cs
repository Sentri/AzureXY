using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureXY.Startup))]
namespace AzureXY
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
