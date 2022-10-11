using Demo.Shared.Models.UserTracking;
using SSP = System.Security.Principal;

namespace Demo.Shared.Interfaces
{
    public interface IBlazorUserService
    {
        event BlazorCircuitsChangedEventHandler CircuitsChanged;
        event BlazorUserRemovedEventHandler UserRemoved;

        void BlazorStartAddUser(SSP.IPrincipal principal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        string UserIsFoundAndAuthenticated(SSP.IPrincipal principal);
        bool UserFoundInDictionary(string userIdentifier);

        string Connect(SSP.IPrincipal principal, string circuitId);
        int Disconnect(string circuitId);
        int? GetUserCircuitsCount(string nameIdentifier);

        bool UserLoggingOut(SSP.IPrincipal principal);

        string ValidateUserIsFoundAndAuthenticated(SSP.IPrincipal principal, string circuitId);
    }
}
