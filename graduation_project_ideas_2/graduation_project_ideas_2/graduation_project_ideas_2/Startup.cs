using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(graduation_project_ideas_2.Startup))]
namespace graduation_project_ideas_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
