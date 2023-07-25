using Newtonsoft.Json.Linq;
using System;
using Ticketvoordeel.Models;
using Ticketvoordeel.Utils;

namespace Ticketvoordeel.Helpers
{
    public static class AuthenticationHelper
    {
        public static string GenerateToken()
        {
            string token = new ApiRequestHelper().PostData(Constants.TurSysApiURL + Constants.TurSysTokenGenerationURL, "grant_type=password&username=" + Constants.UserName + "&password=" + Constants.Password + "&client_id=" + Constants.ClientId + "&client_secret=" + Constants.ClientSecret + "").Result;
            dynamic data = JObject.Parse(token);
            TursysAuthentication tursysAuthentication = new TursysAuthentication();
            tursysAuthentication = data.ToObject<TursysAuthentication>();
            return tursysAuthentication.access_token.Trim('"');
        }        
    }
}
