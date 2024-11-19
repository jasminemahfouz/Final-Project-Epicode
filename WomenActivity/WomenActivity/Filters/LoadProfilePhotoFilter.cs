using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using WomenActivity.Models;
using Microsoft.Extensions.Logging;

namespace WomenActivity.Filters
{
    public class LoadProfilePhotoFilter : IAsyncActionFilter
    {
        private readonly WomenActivityDbContext _context;
        private readonly ILogger<LoadProfilePhotoFilter> _logger;

        public LoadProfilePhotoFilter(WomenActivityDbContext context, ILogger<LoadProfilePhotoFilter> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            if (controller != null && controller.User.Identity.IsAuthenticated)
            {
                var userIdClaim = controller.User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    var session = context.HttpContext.Session;
                    var profilePhoto = session.GetString("ProfilePhoto");

                    if (string.IsNullOrEmpty(profilePhoto))
                    {
                        // Retrieve from database only if not found in session
                        var userProfile = await _context.UserProfiles.FindAsync(userId);
                        if (userProfile != null)
                        {
                            profilePhoto = !string.IsNullOrEmpty(userProfile.ProfilePhoto)
                                ? userProfile.ProfilePhoto
                                : "/images/profiles/photo1.jpg";

                            session.SetString("ProfilePhoto", profilePhoto); // Store in session
                        }
                    }

                    controller.ViewData["ProfilePhoto"] = profilePhoto;
                }
            }

            await next();
        }
    }
}
