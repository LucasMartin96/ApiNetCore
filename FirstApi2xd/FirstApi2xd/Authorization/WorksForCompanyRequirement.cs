using Microsoft.AspNetCore.Authorization;

namespace FirstApi2xd.Authorization
{
    public class WorksForCompanyRequirement : IAuthorizationRequirement
    {
        public string DomainMain { get; }
        public WorksForCompanyRequirement(string domainMain)
        {
            DomainMain = domainMain;
        }
    }
}