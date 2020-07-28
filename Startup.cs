using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(gs_blog_cf.Startup))]
namespace gs_blog_cf
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
