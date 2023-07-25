using System;
using System.Collections.Generic;
using System.IO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using System.Linq;
using System.Text;
using MimeKit;

namespace Ticketvoordeel.Controllers
{
    public class ExtraController : Controller
    {
        private IRepositoryWrapper _repository;
        private IWebHostEnvironment _hostingEnvironment;

        public ExtraController(IRepositoryWrapper repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _hostingEnvironment = environment;
        }

        [Route("/parking/quoting")]
        [AllowAnonymous]
        public JsonResult GetQuoting([FromBody]ParkingQuoteRequest parkingQuoteRequest)
        {
            try
            {
                return Json(new Response { Data = JObject.Parse(new ApiRequestHelper().PostData(Utils.Constants.ParkingURL + "quoting", JsonConvert.SerializeObject(parkingQuoteRequest)).Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet("getVisitorCount")]
        [AllowAnonymous]
        public JsonResult GetVisitorCount()
        {
            try
            {
                using (var sr = new StreamReader(Path.Combine(_hostingEnvironment.WebRootPath + "\\visitor_count\\visitor_count.txt")))
                {
                    int counter = Convert.ToInt32(sr.ReadToEnd());
                    counter += _repository.BookingRepository.GetAllBooking().Count();
                    DateTime fromDate = new DateTime(2021, 01, 01);
                    DateTime currentDate = DateTime.Now;
                    double days = (currentDate - fromDate).TotalDays;
                    days = (days * 24 * 60);
                    days /= 3;
                    counter += Convert.ToInt32(days);

                    return Json(counter);
                }
            }
            catch
            {
                return Json("245359");
            }
        }

        [HttpGet("testmail")]
        public JsonResult SendTestMail()
        {
            TextPart data;
            StringBuilder sb = new StringBuilder();
            sb.Append("Test mail");
            data = new TextPart("html")
            {
                Text = sb.ToString()
            };
            new EmailHelper().SendMail("ekrem@ticketvoordeel.nl", "Test mail", data);
            new EmailHelper().SendMail("developer@adotravel.nl", "Test mail", data);
            return Json("success");
        }
    }
}