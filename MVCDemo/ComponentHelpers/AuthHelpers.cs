using MVCDemo.Models.UserInfo;
using Demo.Shared.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using CUS = MVCDemo.Services;
using SSC = System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace MVCDemo.ComponentHelpers
{
    internal static class AuthHelpers
    {
        internal static async Task<SSC.ClaimsPrincipal> GetAuthenticationStateUserAsync(this AuthenticationStateProvider authenticationStateProvider)
        {
            SSC.ClaimsPrincipal user = null;
            AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState != null)
            {
                user = authState.User;
            }
            return user;
        }

        internal static async Task<UserInfoResult> HandleUserInfoAtStart(
            this AuthenticationStateProvider authenticationStateProvider,
            CUS.CircuitHandlerService circuitHandler,
            IBlazorUserService blazorUserService
            )
        {
            UserInfoResult result = new UserInfoResult();

            int step = 1;
            try
            {
                AuthenticationState authState = await authenticationStateProvider.GetAuthenticationStateAsync();
                if (authState != null)
                {
                    result.User = authState.User;
                }
                step++;

                if ((result.User != null) && (result.User.Identity != null)
                    && result.User.Identity.IsAuthenticated)
                {
                    result.UserIsAuthenticated = true;
                    result.UserName = !string.IsNullOrWhiteSpace(result.User.Identity.Name)
                        ? result.User.Identity.Name : "?";

                    result.CircuitId = circuitHandler.CircuitId;

                    step++;
                    if (!string.IsNullOrWhiteSpace(result.CircuitId))
                    {
                        result.UserIdentifier = blazorUserService.Connect(result.User, result.CircuitId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                result.LogErrorMessage = $"HandleUserInfoAtStart step={step} {ex.GetType().Name}: {ex.Message}";
            }

            return result;
        }

    }
}
