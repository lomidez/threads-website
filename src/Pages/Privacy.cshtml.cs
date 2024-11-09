using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Represents the model for the Privacy page.
    /// Handles data and operations related to the Privacy view.
    /// </summary>
    public class PrivacyModel : PageModel
    {
        // Logger instance for logging any messages related to the Privacy page
        private readonly ILogger<PrivacyModel> _logger;

        /// <summary>
        /// Initializes a new instance of the PrivacyModel class with a logger.
        /// </summary>
        /// <param name="logger">The logger used for logging diagnostic information.</param>
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles HTTP GET requests for the Privacy page.
        /// This method is called when the page is accessed via a GET request.
        /// </summary>
        public void OnGet()
        {
            // Currently, no specific logic is implemented for the GET request.
            // This method can be expanded if any server-side data needs to be prepared for the Privacy view.
        }
    }
}
