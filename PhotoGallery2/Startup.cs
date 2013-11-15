using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotoGallery2.Startup))]
namespace PhotoGallery2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
