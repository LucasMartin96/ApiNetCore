using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FirstApi2xd.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User == null ? string.Empty : httpContext.User.Claims.Single(x => x.Type == "id").Value;
        } 
    }
}