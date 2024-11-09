using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Represents the model for the Error page, handling error information and logging.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current HTTP request.
        /// Used for tracking and debugging purposes.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Determines whether to display the Request ID on the Error page.
        /// Returns true if the Request ID is not null or empty.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Logger instance for logging error information and diagnostics
        private readonly ILogger<ErrorModel> _logger;

        /// <summary>
        /// Initializes a new instance of the ErrorModel class with a logger.
        /// </summary>
        /// <param name="logger">The logger used for logging error messages.</param>
        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles HTTP GET requests for the Error page.
        /// Retrieves the Request ID from the current activity or the HTTP context trace identifier.
        /// </summary>
        public void OnGet()
        {
            // Set the RequestId using the current activity's ID or the HTTP context's trace identifier
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
