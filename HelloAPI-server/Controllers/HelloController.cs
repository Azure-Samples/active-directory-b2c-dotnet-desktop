using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace HelloAPI_server.Controllers
{

    public class HelloController : ApiController
    {
        // OWIN auth middleware constants
        public const string scopeElement = "http://schemas.microsoft.com/identity/claims/scope";
        public const string objectIdElement = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        [Authorize]
        [Route("hello")]
        [HttpGet]
        public string Hello()
        {
            HasRequiredScopes("read");
            string name = ClaimsPrincipal.Current.FindFirst("name").Value;
            return "Hello there! " + name;
        }

        // Validate to ensure the necessary scopes are present.
        private void HasRequiredScopes(String permission)
        {
            if (!ClaimsPrincipal.Current.FindFirst(scopeElement).Value.Contains(permission))
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ReasonPhrase = $"The Scope claim does not contain the {permission} permission."
                });
            }
        }
    }
}
