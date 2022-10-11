using Ardalis.GuardClauses;
using Demo.Infrastructure.Helpers;
using System.Collections.Concurrent;
using SSP = System.Security.Principal;

namespace Demo.Infrastructure.Models
{
    internal class BlazorUserInfo
    {
        internal BlazorUserInfo(string nameIdentifier, SSP.IPrincipal principal)
        {
            Guard.Against.Null(principal, nameof(principal));
            Principal = principal;
            Guard.Against.Null(nameIdentifier, nameof(nameIdentifier));
            NameIdentifier = nameIdentifier;
            Circuits = new ConcurrentDictionary<string, string>();
            Username = principal.GetName();
        }

        public ConcurrentDictionary<string, string> Circuits { get; private set; }
        internal string NameIdentifier { get; private set; }
        internal SSP.IPrincipal Principal { get; private set; }
        internal string Username { get; private set; }
    }
}
