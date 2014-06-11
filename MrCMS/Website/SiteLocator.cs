using System.Threading.Tasks;
using Microsoft.Owin;

namespace MrCMS.Website
{
    public class SiteLocator : OwinMiddleware
    {
        public SiteLocator(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);
        }
    }
}