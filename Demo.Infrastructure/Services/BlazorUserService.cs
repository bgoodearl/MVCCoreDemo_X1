using Ardalis.GuardClauses;
using Demo.Shared.Interfaces;
using Demo.Shared.Models.UserTracking;
using Demo.Infrastructure.Helpers;
using Demo.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Principal;
using SSC = System.Security.Claims;
using System;
using System.Collections.Generic;

namespace Demo.Infrastructure.Services
{
    public class BlazorUserService : IBlazorUserService
    {
        public BlazorUserService(ILogger<BlazorUserService> logger)
        {
            Guard.Against.Null(logger, nameof(logger));
            Logger = logger;
            BlazorUsersDict = new ConcurrentDictionary<string, BlazorUserInfo>();
        }

        public event BlazorCircuitsChangedEventHandler CircuitsChanged;
        public event BlazorUserRemovedEventHandler UserRemoved;


        #region read-only variables

        private ConcurrentDictionary<string, BlazorUserInfo> BlazorUsersDict { get; }
        ILogger<BlazorUserService> Logger { get; }

        #endregion read-only variables

        void OnCircuitsChanged()
        {
            var args = new BlazorCircuitsChangedEventArgs
            {
                CircuitsChangedTime = DateTime.Now
            };
            CircuitsChanged?.Invoke(this, args);
        }

        void OnUserRemoved()
        {
            var args = new BlazorUserRemovedEventArgs
            {
                UserRemovedTime = DateTime.Now
            };
            UserRemoved?.Invoke(this, args);
        }

        public void BlazorStartAddUser(IPrincipal principal)
        {
            Guard.Against.Null(principal, nameof(principal));
            Guard.Against.Null(principal.Identity, nameof(principal.Identity));
            if (!principal.Identity.IsAuthenticated)
            {
                Logger.LogError("BlazorUserSvc.BlazorStartAddUser - Identity not authenticated");
            }
            else
            {
                SSC.ClaimsPrincipal cp = principal as SSC.ClaimsPrincipal;
                Guard.Against.Null(cp, nameof(cp));
                SSC.Claim nameIdentifierClaim = cp.GetClaim(SSC.ClaimTypes.NameIdentifier);
                if ((nameIdentifierClaim == null) || string.IsNullOrWhiteSpace(nameIdentifierClaim.Value))
                    throw new InvalidOperationException("NameIdentifier Claim not found or empty");

                if (!BlazorUsersDict.ContainsKey(nameIdentifierClaim.Value))
                {
                    BlazorUserInfo userInfo = new BlazorUserInfo(nameIdentifierClaim.Value, principal);
                    BlazorUsersDict[nameIdentifierClaim.Value] = userInfo;
                }
                else
                {
                    //***TODO: Bump count??
                }
            }
        }

        //private string? GetNameIdentifier(IPrincipal principal)
        //{
        //    if ((principal != null) && (principal.Identity != null) && principal.Identity.IsAuthenticated)
        //    {
        //        SSC.ClaimsPrincipal? cp = principal as SSC.ClaimsPrincipal;
        //        if (cp != null)
        //        {
        //            SSC.Claim? identiferClaim = cp.GetClaim(SSC.ClaimTypes.NameIdentifier);
        //            if (identiferClaim != null)
        //                return identiferClaim.Value;
        //        }
        //    }
        //    return null;
        //}

        private BlazorUserInfo UserIsFoundAndAuthenticatedInternal(IPrincipal principal)
        {
            Guard.Against.Null(principal, nameof(principal));
            Guard.Against.Null(principal.Identity, nameof(principal.Identity));
            if (!principal.Identity.IsAuthenticated) return null;
            SSC.ClaimsPrincipal cp = principal as SSC.ClaimsPrincipal;
            Guard.Against.Null(cp, nameof(cp));
            SSC.Claim nameIdentifierClaim = cp.GetClaim(SSC.ClaimTypes.NameIdentifier);
            if ((nameIdentifierClaim == null) || string.IsNullOrWhiteSpace(nameIdentifierClaim.Value))
                throw new InvalidOperationException("NameIdentifier Claim not found or empty");
            if (!BlazorUsersDict.ContainsKey(nameIdentifierClaim.Value))
                return null;

            BlazorUserInfo userInfo = BlazorUsersDict[nameIdentifierClaim.Value];
            return (userInfo.Principal.Identity != null && userInfo.Principal.Identity.IsAuthenticated)
                ? userInfo : null;
        }

        public string UserIsFoundAndAuthenticated(IPrincipal principal)
        {
            Guard.Against.Null(principal, nameof(principal));

            BlazorUserInfo userInfo = UserIsFoundAndAuthenticatedInternal(principal);
            if (userInfo != null)
            {
                return userInfo.NameIdentifier;
            }
            return null;
        }

        public string ValidateUserIsFoundAndAuthenticated(IPrincipal principal, string circuitId)
        {
            Guard.Against.Null(principal, nameof(principal));
            Guard.Against.NullOrEmpty(circuitId, nameof(circuitId));

            BlazorUserInfo userInfo = UserIsFoundAndAuthenticatedInternal(principal);

            if (userInfo != null)
            {
                if (userInfo.Circuits.ContainsKey(circuitId))
                {
                    return userInfo.NameIdentifier;
                }
            }
            return null;
        }

        public string Connect(IPrincipal principal, string circuitId)
        {
            Guard.Against.Null(principal, nameof(principal));
            Guard.Against.NullOrEmpty(circuitId, nameof(circuitId));

            BlazorUserInfo userInfo = UserIsFoundAndAuthenticatedInternal(principal);
            if (userInfo != null)
            {
                if (userInfo.Circuits.ContainsKey(circuitId))
                {
                    Logger.LogWarning($"BlazorUserSvc-Connect Existing user [{userInfo.Username}] already has CircuitId [{circuitId}]");
                }
                else
                {
                    userInfo.Circuits[circuitId] = userInfo.NameIdentifier;
                    OnCircuitsChanged();
                }
                return userInfo.NameIdentifier;
            }
            return null;
        }

        public int Disconnect(string circuitId)
        {
            //***TODO: In THEORY, there should only be ONE entry with the specified circuitId!!

            List<BlazorUserInfo> usersWithCircuitRemoved = new List<BlazorUserInfo>();
            foreach (var kv in BlazorUsersDict)
            {
                if (kv.Value.Circuits.ContainsKey(circuitId))
                {
                    string nameIdentifierFromCircuitRemoved;
                    if (kv.Value.Circuits.TryRemove(circuitId, out nameIdentifierFromCircuitRemoved))
                    {
                        if (kv.Value.NameIdentifier != nameIdentifierFromCircuitRemoved)
                        {
                            Logger.LogWarning($"BlazorUserSvc-Disconnect User [{kv.Value.Username}] identifier [{kv.Value.NameIdentifier}] does not match [{nameIdentifierFromCircuitRemoved}] from circuit removed, but has CircuitId [{circuitId}]");
                        }
                        usersWithCircuitRemoved.Add(kv.Value);
                    }
                }
            }
            List<string> nameIdentifiersFromUsersWithCircuitRemoved = new List<string>();

            foreach (BlazorUserInfo userInfo in usersWithCircuitRemoved)
            {
                if (!nameIdentifiersFromUsersWithCircuitRemoved.Contains(userInfo.NameIdentifier))
                    nameIdentifiersFromUsersWithCircuitRemoved.Add(userInfo.NameIdentifier);

                if (userInfo.Circuits.Count == 0)
                {
                    //***TODO: do we do something if all circuits are removed?
                }
            }

            if (usersWithCircuitRemoved.Count > 0)
            {
                OnCircuitsChanged();
            }
            return usersWithCircuitRemoved.Count;
        }

        public int? GetUserCircuitsCount(string nameIdentifier)
        {
            if (BlazorUsersDict.ContainsKey(nameIdentifier))
            {
                BlazorUserInfo userInfo = BlazorUsersDict[nameIdentifier];
                if (userInfo != null)
                {
                    return userInfo.Circuits.Count;
                }
            }
            return null;
        }

        public bool UserFoundInDictionary(string userIdentifier)
        {
            if (!string.IsNullOrWhiteSpace(userIdentifier) && BlazorUsersDict.ContainsKey(userIdentifier))
            {
                BlazorUserInfo userInfo = BlazorUsersDict[userIdentifier];
                if (userInfo != null)
                    return true;
            }
            return false;
        }

        public bool UserLoggingOut(IPrincipal principal)
        {
            if (principal != null)
            {
                SSC.ClaimsPrincipal cp = principal as SSC.ClaimsPrincipal;
                if (cp != null)
                {
                    SSC.Claim nameIdentifierClaim = cp.GetClaim(SSC.ClaimTypes.NameIdentifier);
                    if ((nameIdentifierClaim != null) && !string.IsNullOrWhiteSpace(nameIdentifierClaim.Value))
                    {
                        if (BlazorUsersDict.ContainsKey(nameIdentifierClaim.Value))
                        {
                            BlazorUserInfo userInfo = BlazorUsersDict[nameIdentifierClaim.Value];
                            if (userInfo != null)
                            {
                                BlazorUserInfo removedUserInfo;
                                bool removed = BlazorUsersDict.TryRemove(nameIdentifierClaim.Value, out removedUserInfo);
                                //***TODO: Anything to do here?
                                //if (removed && removedUserInfo != null)
                                //{

                                //}
                                //else
                                //{

                                //}
                                OnUserRemoved();
                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
