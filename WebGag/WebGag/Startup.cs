using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(WebGag.Startup))]
namespace WebGag
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Database.SetInitializer<ColmanGag.DBContext.GagsDbContext>(null);
            ConfigureAuth(app);
        }
    }
}
