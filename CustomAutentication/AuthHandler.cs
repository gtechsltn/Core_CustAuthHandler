using Core_CustAuthHandler.Models;
using Core_CustAuthHandler.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Core_CustAuthHandler.CustomAutentication
{
    /// <summary>
    /// The Handler Class
    /// </summary>
    public class AuthHandler : AuthenticationHandler<AuthSchemeOptions>
    {

        private readonly IUserService userService;

        public AuthHandler(IUserService userService, IOptionsMonitor<AuthSchemeOptions> optionsMonitor,ILoggerFactory logger, UrlEncoder encoder) :base(optionsMonitor, logger, encoder)   
        {
            this.userService = userService;
        }

        /// <summary>
        /// Method to Handle the Request for Authentication
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            // 1. Check if the Autentication Header is present ot not

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization Header is not present in the request");

            // 2. Read the Header value
            var headerAuthValues = Request.Headers["Authorization"];
            // 3. Now parse these values and check for authentication
            try
            {
                //3.a. Parse 
                var authValues = AuthenticationHeaderValue.Parse(headerAuthValues);
                //3.b. Convert from Base 64 String
                var authValuesBytes = Convert.FromBase64String(authValues.Parameter);
                // 3.c. Now Split by :
                var authCredentials = Encoding.UTF8.GetString(authValuesBytes).Split(new[] { ':' }, 2);
                // 3.d. Read the UserName as Password
                User? authUser = new User() 
                {
                  UserName= authCredentials[0],
                  Password= authCredentials[1]
                };

                bool isAuthenticate = await userService.AuthenticateUserAsync(authUser);

                if(!isAuthenticate)
                    return AuthenticateResult.Fail("User Autheitcation Failed");

                // 3.e. Lets Claim the Authentication Ticket
                // Read the UserId from the UserName
                var id = (await userService.GetUserAsync(authUser.UserName)).UserId;
                // Set the Claims
                var authClaims = new[] {
                        new Claim("Id", id.ToString()),
                        new Claim("Name", authUser.UserName),
                };
                var autIdentity = new ClaimsIdentity(authClaims, Scheme.Name);
                var authPrincipal = new ClaimsPrincipal(autIdentity);
                var authTicket = new AuthenticationTicket(authPrincipal, Scheme.Name);
                // Return the Authentication Ticket
                return AuthenticateResult.Success(authTicket);

            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Invvalid Authorization header ");
            }
        }
    }
}
