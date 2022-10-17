using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MVCDemo.Models.Configuration;
using System.Threading.Tasks;
using static MVCDemo.Models.MVCDemoDefs;

namespace MVCDemo.Controllers
{
    public class MDControllerBase : Controller
    {
        #region Constructors
        public MDControllerBase(IOptions<AppSettings> appSettings,
            ILogger<MDControllerBase> baseLogger,
            IHttpContextAccessor httpContextAccessor)
        {
            Guard.Against.Null(appSettings, nameof(appSettings));
            AppSettings = appSettings.Value;
            Guard.Against.Null(baseLogger, nameof(baseLogger));
            BaseLogger = baseLogger;
            Guard.Against.Null(httpContextAccessor, nameof(httpContextAccessor));
            HttpContextAccessor = httpContextAccessor;
        }
        #endregion Constructors


        #region Read Only variables

        protected AppSettings AppSettings { get; }
        private ILogger<MDControllerBase> BaseLogger { get; }
        protected IHttpContextAccessor HttpContextAccessor { get; }

        #endregion Read Only variables


        #region Token Support

        protected async Task<string> GetApiTokenAsync()
        {
            if ((HttpContextAccessor != null) && (HttpContextAccessor.HttpContext != null))
            {
                string accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync(MDClaimTypes.access_token);
                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    BaseLogger.LogWarning($"GetApiTokenAsync - no {MDClaimTypes.access_token} token found in claims");
                }
                return accessToken;
            }
            return string.Empty;
        }

        protected string GetApiUrlRoot()
        {
            string apiUrlRoot = AppSettings.apiUrlRoot;
            if (string.IsNullOrWhiteSpace(apiUrlRoot))
            {
                if ((HttpContextAccessor != null) && (HttpContextAccessor.HttpContext != null)
                                        && (HttpContextAccessor.HttpContext.Request != null))
                {
                    apiUrlRoot = $"{HttpContextAccessor.HttpContext.Request.Scheme}://{HttpContextAccessor.HttpContext.Request.Host}/api";
                }
            }
            return apiUrlRoot;
        }

        #endregion Token Support
    }
}
