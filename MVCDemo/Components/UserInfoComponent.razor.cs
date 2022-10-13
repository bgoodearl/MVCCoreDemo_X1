using MVCDemo.ComponentHelpers;
using MVCDemo.Models.UserInfo;
using Demo.Shared.Interfaces;
using Demo.Shared.Models.UserTracking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using MDS = MVCDemo.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSC = System.Security.Claims;

namespace MVCDemo.Components
{
    public partial class UserInfoComponent : ComponentBase, IDisposable
    {
        [Parameter] public bool ShowDiagnostics { get; set; }

        protected string CircuitId { get; set; }
        protected string Message { get; set; }

        private string UserIdentifier { get; set; }
        private bool UserAuthenticated { get; set; }


        #region Injected items

        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        protected AuthenticationStateProvider AuthStateProvider { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        IBlazorUserService BlazorUserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

        //CircuitHandler passed in from where it's injected in the UI
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        MDS.CircuitHandlerService CircuitHandler { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

        [Inject]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
        protected ILogger<UserInfoComponent> Logger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

        #endregion Injected items

        public string MyUsername;
        public int? MyUserCircuitCount;
        public string MyCircuitMessage = "";
        public string UserRemovedMessage = "";

        protected List<ClaimInfo> ClaimItems { get; private set; } = new List<ClaimInfo>();



        #region UserService events

        private void BlazorUserService_CircuitsChanged(object sender, BlazorCircuitsChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserIdentifier))
            {
                MyUserCircuitCount = BlazorUserService.GetUserCircuitsCount(UserIdentifier);
                if (MyUserCircuitCount.HasValue)
                {
                    InvokeAsync(() => StateHasChanged());
                }
            }
        }

        private void BlazorUserService_UserRemoved(object sender, BlazorUserRemovedEventArgs e)
        {
            int? PrevUserCircuitCount = MyUserCircuitCount;
            if (!string.IsNullOrWhiteSpace(UserIdentifier)
                && !BlazorUserService.UserFoundInDictionary(UserIdentifier))
            {
                UserAuthenticated = false;
                MyUserCircuitCount = BlazorUserService.GetUserCircuitsCount(UserIdentifier);
                UserRemovedMessage = $"User Removed - Circuit Count = {MyUserCircuitCount}, Prev Count = {PrevUserCircuitCount}";
                InvokeAsync(() => StateHasChanged());
            }
        }

        #endregion UserService events


        #region Events

        protected override async Task OnInitializedAsync()
        {
            CircuitHandler = (MDS.CircuitHandlerService)BlazorCircuitHandler;

            UserInfoResult userInfo = await AuthStateProvider.HandleUserInfoAtStart(CircuitHandler, BlazorUserService);
            if (!string.IsNullOrWhiteSpace(userInfo.LogErrorMessage))
            {
                Logger.LogError(userInfo.Exception, userInfo.LogErrorMessage);
            }
            else if (userInfo.Exception != null)
            {
                Logger.LogError(userInfo.Exception, "HandleUserInfoAtStart {0}: {1}",
                    userInfo.Exception.GetType().Name,
                    userInfo.Exception.Message);
            }
            CircuitId = userInfo.CircuitId;

            MyCircuitMessage = $"My Circuit ID = {CircuitId}";

            if ((userInfo.User == null) || !userInfo.UserIsAuthenticated)
            {
                Message = "User Not Authenticated";
            }
            else
            {
                UserAuthenticated = userInfo.UserIsAuthenticated;
                MyUsername = userInfo.UserName;


                if (!string.IsNullOrWhiteSpace(CircuitId) && !string.IsNullOrWhiteSpace(userInfo.UserIdentifier))
                {
                    UserIdentifier = userInfo.UserIdentifier;
                    try
                    {
                        MyUserCircuitCount = BlazorUserService.GetUserCircuitsCount(UserIdentifier);
                        BlazorUserService.CircuitsChanged += BlazorUserService_CircuitsChanged;
                        BlazorUserService.UserRemoved += BlazorUserService_UserRemoved;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "UserInfoComponent.OnInitializedAsync {0}: {1}",
                            ex.GetType().Name, ex.Message);
                    }
                }

                ClaimItems = new List<ClaimInfo>();
                foreach(var claim in userInfo.User.Claims)
                {
                    string claimValue = claim.Value;
                    if (!string.IsNullOrWhiteSpace(claim.ValueType)
                        && claim.ValueType == SSC.ClaimValueTypes.Integer)
                    {
                        long unixDateTime;
                        if (!string.IsNullOrWhiteSpace(claimValue) && long.TryParse(claimValue, out unixDateTime))
                        {
                            try
                            {
                                DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixDateTime);
                                claimValue = $"{claimValue} -- {dto:yyyy-MM-dd h:mm:ss tt} (UTC)";
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                int foo = 1;
#endif
                            }
                        }
                    }
                    ClaimItems.Add(new ClaimInfo
                    {
                        ClaimType = claim.Type,
                        ClaimValue = claimValue,
                        ClaimValueType = claim.ValueType
                    });
                }
            }

        }

        #endregion Events



        #region IDisposable

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(UserIdentifier))
            {
                BlazorUserService.CircuitsChanged -= BlazorUserService_CircuitsChanged;
                BlazorUserService.UserRemoved -= BlazorUserService_UserRemoved;
            }
        }

        #endregion IDisposable

    }
}
