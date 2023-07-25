using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;

namespace Ticketvoordeel.Controllers
{
    public class HelperController : Controller
    {
        [Route("/nationality/list")]
        [HttpGet]
        public JsonResult GetNationalityList()
        {
            try
            {
                return Json(new Response { Data = JsonConvert.DeserializeObject(new ApiRequestHelper().Get("/api/customer/nation/?id=&includetranslations=").Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [Route("/identity/list")]
        [HttpGet]
        public JsonResult GetIdentityList()
        {
            try
            {
                return Json(new Response { Data = JsonConvert.DeserializeObject(new ApiRequestHelper().Get("/api/basic/Identity").Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [Route("/offers/list")]
        [HttpGet]
        public JsonResult GetOffersList()
        {
            try
            {
                return Json(new Response { Data = "[{'Id':'1','Name':'Start','Amount':'0'},{'Id':'2','Name':'Compleet','Amount':'5'},{'Id':'3','Name':'Max','Amount':'10'}]", Message = "success", Status = true });
            }
            catch (Exception)
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [Route("/cancellationinsurance/premium")]
        [HttpPost]
        public JsonResult GetCancellationInsurancePremium([FromBody]CancellationInsuranceRequest cancellationInsuranceRequest)
        {
            try
            {
                return Json(new Response { Data = new InsuranceHelper().CheckCancellationInsurancePremium(cancellationInsuranceRequest.StartDate, cancellationInsuranceRequest.Amount), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }
    }
}