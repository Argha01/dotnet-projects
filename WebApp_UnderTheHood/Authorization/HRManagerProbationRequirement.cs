using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization
{
    public class HRManagerProbationRequirement : IAuthorizationRequirement
    {
        public int probationperiod { get; }
        public HRManagerProbationRequirement(int probationperiod)
        {
            this.probationperiod = probationperiod;
        }
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmploymentDate"))
                return Task.CompletedTask;

            if (DateTime.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == "EmploymentDate")?.Value, out DateTime employmentdate))
            {
                if ((DateTime.UtcNow - employmentdate).Days > requirement.probationperiod)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
