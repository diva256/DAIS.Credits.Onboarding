using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Credits.Onboarding.WebAPI.Controllers
{
    public class CreditRequestStatusController : DAIS.Credits.Onboarding.WebAPI.Definition.Controllers.CreditRequestStatusController
    {
        public override async Task<ActionResult<string>> SendUpdateStatusNotification(int status)
        {
            var client = new DAIS.Interop.POSCredits.Onboarding.Client(new ClientOptions());
            var result = await client.Status_phpAsync(new Interop.POSCredits.Onboarding.UpdateStatusNotificationRequest()
            {
                OrderId = "0000000000000",
                StatusId = status
            });
            return Ok(result.Status);
        }
    }

    class ClientOptions : DAIS.Interop.POSCredits.Onboarding.IClientOptions
    {
        public string BaseAddress => "https://dsk.avalontest.eu";
    }
}
