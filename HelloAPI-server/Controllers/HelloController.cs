using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HelloAPI_server.Controllers
{
    public class HelloController : ApiController
    {
        [Route("hello")]
        [HttpGet]
        public string Hello()
        {
            return "Hello there!";
        }
    }
}
