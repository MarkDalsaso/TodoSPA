using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private TodoContext db = new TodoContext();

        public class vmUserName
        {
            public int RecId { set; get; }
            public string FirstName { set; get; }
            public string LastName { set; get; }
        }

        // Get name used for changing the user via dropdown
        [HttpGet]
        [Route("names")]
        public HttpResponseMessage GetUserNames()
        {
            IQueryable<vmUserName> names = db
                .tblUsers
                .Where(r => r.Active == true)
                .Select(r => new vmUserName
                {
                    RecId = r.RecId,
                    FirstName = r.FirstName,
                    LastName = r.LastName
                })
                .OrderBy(o => o.LastName);

            var response = Request.CreateResponse(HttpStatusCode.OK, names);
            return response;
        }

    }
}
