using IdentityModel.Client;
using MVCDemo.Models.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCDemo.AuthHelpers
{
    public class IdSvrTokenHelper
    {
        internal static async Task<TokenResponse> GetToken(IdentityServerOptions idSverSettings)
        {
            HttpClient client = new HttpClient();
            TokenResponse response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{idSverSettings.Authority}/connect/token",

                ClientId = idSverSettings.ClientId,
                ClientSecret = idSverSettings.ClientSecret,
                Scope = "user"
            });
            return response;
        }
    }
}
