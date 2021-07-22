using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DAIS.Credits.Onboarding.WebAPI.Definition.Controllers
{
    [Route("creditRequestStatus")]
    [Produces("application/json")]
    public abstract class CreditRequestStatusController : ControllerBase
    {
        [HttpPost("updateStatusNotification/{orderId}")]
        public virtual Task<ActionResult<String>> SendUpdateStatusNotification(int status) => null;
    }
}
