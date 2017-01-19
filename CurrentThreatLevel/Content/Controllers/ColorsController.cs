using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CurrentThreatLevel.Content.Controllers
{
    public class ColorsController : ApiController
    {
        private static CurrentThreatLevel.Repositories.ColorRepository _colorRepository = new CurrentThreatLevel.Repositories.ColorRepository();

        // GET: api/Colors
        public IHttpActionResult Get()
        {
            return Ok(ColorsController._colorRepository.getColors());
        }
    }
}
