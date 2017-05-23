using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Budget_Planner.Startup))]
namespace Budget_Planner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
