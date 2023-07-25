using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using MimeKit;
using Newtonsoft.Json;
using NLog.Time;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.Booking;
using Ticketvoordeel.Models.BookRequest;
using Ticketvoordeel.Utils;

namespace Ticketvoordeel.Controllers
{
    public class AdminDashboardController : Controller
    {
        private IRepositoryWrapper _repository;
        private IWebHostEnvironment _hostingEnvironment;


        public AdminDashboardController(IRepositoryWrapper repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _hostingEnvironment = environment;
        }

        #region data

        [AllowAnonymous]
        [Route("/admin/savelastminutedeals")]
        [HttpPost]
        public JsonResult SaveLastMinuteDeals(LastMinuteDealsRequest lastMinuteDealsRequest)
        {
            try
            {
                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();
                Ticketvoordeel.Models.LastMinuteDeals lastMinuteDeals = new LastMinuteDeals();
                lastMinuteDeal.PoolSearchRequest = JsonConvert.SerializeObject(GenerateLastMinuteDeals(lastMinuteDeals, lastMinuteDealsRequest));
                if (lastMinuteDealsRequest.Id > 0)
                {
                    lastMinuteDeal = _repository.LastMinuteDeals.GetLastMinuteDealById(lastMinuteDealsRequest.Id);
                }
                lastMinuteDeal.Id = lastMinuteDealsRequest.Id;
                lastMinuteDeal.Airline = lastMinuteDealsRequest.Airline;
                lastMinuteDeal.ArrivalTime = lastMinuteDealsRequest.ArrivalTime;
                lastMinuteDeal.ColumnNumber = lastMinuteDealsRequest.ColumnNumber;
                lastMinuteDeal.CreationTime = DateTime.Now;
                lastMinuteDeal.UpdatedTime = DateTime.Now;
                lastMinuteDeal.DepartureTime = lastMinuteDealsRequest.DepartureTime;
                lastMinuteDeal.FromLocation = lastMinuteDealsRequest.FromAirportCode;                
                lastMinuteDeal.IsActive = true;
                lastMinuteDeal.Price = lastMinuteDealsRequest.Price;
                lastMinuteDeal.Title = Enum.GetName(typeof(Constants.LastMinuteDeals), lastMinuteDealsRequest.ColumnNumber);
                lastMinuteDeal.ToLocation = lastMinuteDealsRequest.ToAirportCode;

                if (lastMinuteDeal.Id > 0)
                {
                    _repository.LastMinuteDeals.UpdateLastMinuteDeal(lastMinuteDeal);
                }
                else
                {
                    _repository.LastMinuteDeals.CreateLastMinuteDeal(lastMinuteDeal);
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpPost("/admin/uploadblogs")]
        public JsonResult UploadBlogs([FromBody]Blog blog)
        {
            try
            {
                if (blog.Id > 0)
                {
                    _repository.BlogRepository.UpdateBlog(blog);
                }
                else
                {
                    _repository.BlogRepository.CreateBlog(blog);
                }
                if (blog.Id > 0)
                {
                    return Json(new Response { Data = blog, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = blog, Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost("/admin/deletelastminutedeals/{id}")]
        public JsonResult deleteLastMinuteDeals(int id)
        {
            try
            {
                LastMinuteDeal LastMinuteDeal = new LastMinuteDeal();
                LastMinuteDeal = _repository.LastMinuteDeals.GetLastMinuteDealById(id);

                if (_repository.LastMinuteDeals.DeleteLastMinuteDeal(LastMinuteDeal))
                {
                    return Json(new Response { Data = string.Empty, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = string.Empty, Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost("validatelogin")]
        public JsonResult ValidateLogin(LoginViewModel loginViewModel)
        {
            try
            {
                if (loginViewModel.UserName == "aliekrem" && loginViewModel.Password == "Ekrem2020@")
                {
                    HttpContext.Session.SetString("LoggedAdmin", JsonConvert.SerializeObject(loginViewModel));
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }
            }
            catch(Exception ex)
            {
                return Json("error");
            }
        }
        public bool CheckAdminLoggedIn()
        {
            try
            {
                if (HttpContext.Session.GetString("LoggedAdmin") != null)
                {
                    var adminDetails = JsonConvert.DeserializeObject<LoginViewModel>(HttpContext.Session.GetString("LoggedAdmin"));
                    if (adminDetails.UserName == "aliekrem" && adminDetails.Password == "Ekrem2020@")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost("/admin/changemediafolder")]
        public JsonResult ChangeMediaFolder()
        {
            try
            {
                var dynamicPage = _repository.DynamicPages.GetAllDynamicPages();

                if(dynamicPage != null)
                {
                    foreach(var items in dynamicPage)
                    {
                        var path = items.Image;
                        if(path != null)
                        {
                            var splitAddress = path.Split("https://ticketvoordeel.s3.eu-central-1.amazonaws.com/blog/");
                            LogToFile.ChangeFolderPath(path, items.Id);
                            //var splitAddress = path.Split(@"D:\ticketvoordeel_backend\Ticketvoordeel\wwwroot/Uploads/blog/");

                            if (splitAddress != null)
                            {
                                var uploads = "https://api.ticketvoordeel.nl" + "/Uploads/blog/" + Convert.ToString(splitAddress[1]);

                                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uploads);
                                request.Method = "HEAD";

                                bool exists;
                                try
                                {
                                    request.GetResponse();
                                    exists = true;
                                }
                                catch
                                {
                                    exists = false;
                                }

                                if (exists)
                                {
                                    var saveImage = _repository.DynamicPages.GetDynamicPageById(items.Id);
                                    saveImage.Image = uploads;
                                    LogToFile.ChangeFolderPath(uploads, saveImage.Id);
                                    _repository.DynamicPages.UpdateDynamicPage(saveImage);
                                }

                            }
                        }
                    }
                }
                return Json("Changed");
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost("/admin/savedynamicpage")]
        public JsonResult SaveDynamicPages(DynamicPage dynamicPage, IFormFile file)
        {
            try
            {
                DynamicPage existingDynamicPage = new DynamicPage();
                if (dynamicPage.Id > 0)
                {
                    existingDynamicPage = _repository.DynamicPages.GetDynamicPageById(dynamicPage.Id);
                    existingDynamicPage.Title = dynamicPage.Title;
                    existingDynamicPage.Name = dynamicPage.Title.Trim().Replace("?", " ").Replace(" ", "-");
                    existingDynamicPage.Content = dynamicPage.Content;
                    existingDynamicPage.ShortDescription = dynamicPage.ShortDescription;
                    existingDynamicPage.CreatedDate = dynamicPage.CreatedDate;
                    existingDynamicPage.FromAirport = dynamicPage.FromAirport;
                    existingDynamicPage.FromAirportCode = dynamicPage.FromAirportCode;
                    existingDynamicPage.MetaDescription = dynamicPage.MetaDescription;
                    existingDynamicPage.MetaKeywords = dynamicPage.MetaKeywords;
                    existingDynamicPage.MetaTitle = dynamicPage.MetaTitle;
                    existingDynamicPage.ToAirport = dynamicPage.ToAirport;
                    existingDynamicPage.ToAirportCode = dynamicPage.ToAirportCode;
                    if (file != null && file.Length > 0)
                    {
                        existingDynamicPage.Image = Constants.S3BaseURLLocal + "blog/" + new S3UploadHelper().UploadObject(file, "blog", _hostingEnvironment).Result;
                    }
                    LogToFile.writeLog("Updating");
                    _repository.DynamicPages.UpdateDynamicPage(existingDynamicPage);
                    LogToFile.writeLog("Updated");
                }
                else
                {
                    dynamicPage.Name = dynamicPage.Title.Replace(" ", "-");
                    dynamicPage.Language = "nl";
                    if (file != null && file.Length > 0)
                    {
                        dynamicPage.Image = Constants.S3BaseURLLocal + "blog/" + new S3UploadHelper().UploadObject(file, "blog", _hostingEnvironment).Result;
                    }
                    _repository.DynamicPages.CreateDynamicPage(dynamicPage);
                }

                if (dynamicPage.Id > 0)
                {
                    return Json("success");
                }
                else
                {
                    return Json(String.Empty);
                }
            }
            catch (Exception ex)
            {
                return Json(String.Empty);
            }
        }

        [HttpGet("/admin/deletedynamicpage/{id}")]
        public JsonResult DeleteDynamicPages(int id)
        {
            try
            {
                DynamicPage dynamicPage = new DynamicPage();
                dynamicPage = _repository.DynamicPages.GetDynamicPageById(id);
                _repository.DynamicPages.DeleteDynamicPage(dynamicPage);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet("/admin/getdynamicpage/{id}")]
        public JsonResult GetDynamicPages(int id)
        {
            try
            {
                DynamicPage dynamicPage = new DynamicPage();
                dynamicPage = _repository.DynamicPages.GetDynamicPageById(id);
                return Json(dynamicPage);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet("/admin/getdynamicpages")]
        public JsonResult GetDynamicPages()
        {
            try
            {
                var query = from items in _repository.DynamicPages.GetAllDynamicPages()
                            select new
                            {
                                Id = items.Id,
                                Title = items.Title,
                                Type = items.Type,
                                FromAirport = items.FromAirport,
                                ToAirport = items.ToAirport,
                            };
                return Json(query);
            }
            catch(Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/admin/deletelastminutedeal/{id}")]
        public JsonResult DeleteLastMinuteDeals(int id)
        {
            try
            {
                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();
                lastMinuteDeal = _repository.LastMinuteDeals.GetLastMinuteDealById(id);
                _repository.LastMinuteDeals.DeleteLastMinuteDeal(lastMinuteDeal);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet("/admin/getlastminutedeal/{id}")]
        public JsonResult Getlastminutedeal(int id)
        {
            try
            {
                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();
                lastMinuteDeal = _repository.LastMinuteDeals.GetLastMinuteDealById(id);
                return Json(lastMinuteDeal);
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet("/admin/getlastminutedeals")]
        public JsonResult Getlastminutedeals()
        {
            try
            {
                var query = from items in _repository.LastMinuteDeals.GetAllLastMinuteDeals()
                            select new
                            {
                                Id = items.Id,
                                ColumnNumber = items.ColumnNumber,
                                Price = items.Price,
                                FromAirport = items.FromLocation,
                                ToAirport = items.ToLocation,
                            };
                return Json(query);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/admin/getbookings")]
        public JsonResult GetBookings()
        {
            try
            {
                var bookings = _repository.BookingRepository.GetAllBooking();
                var users = _repository.Profile.GetAllProfiles();

                var result = from b in bookings
                             join u in users
                             on b.UserId equals u.Id
                             select new
                             {
                                 u.Name,
                                 u.Mobile,
                                 u.Email,
                                 CreationTime = b.CreationTime.ToString("dd/MM/yyyy"),
                                 b.IsRoundTrip,
                                 b.DeparturePnr,
                                 b.ReturnPnr,
                                 b.TotalAmount,
                                 b.PaidAmount
                             };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/admin/getcredits")]
        public JsonResult GetCredits()
        {
            try
            {
                var users = _repository.Profile.GetAllProfiles();
                List<CreditListing> creditListings = new List<CreditListing>();
                CreditListing creditListing;
                foreach (var user in users)
                {
                    int creditsReceived = 0, creditsApplied = 0;
                    creditListing = new CreditListing();
                    var bookings = _repository.BookingRepository.GetAllBooking().Where(c => c.UserId == user.Id);
                    foreach (var booking in bookings)
                    {
                        creditsReceived += booking.CreditReceived;
                        creditsApplied += booking.CreditApplied;
                    }

                    creditListing.UserName = user.Name;
                    creditListing.Phone = user.Mobile;
                    creditListing.Email = user.Email;
                    creditListing.CreditsReceived = creditsReceived.ToString();
                    creditListing.CreditsApplied = creditsApplied.ToString();
                    creditListing.CreditsAvailable = (creditsReceived - creditsApplied).ToString();
                    creditListings.Add(creditListing);
                }

                return Json(creditListings);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/admin/gettransactions")]
        public JsonResult GetTransactions()
        {
            var transactions = _repository.PaymentTransactionRepository.GetAllPaymentTransaction();

            return Json
                (from items in transactions
                 orderby items.Id descending
                 where items.Status == "Pending"
                    select new
                    {
                        name = items.Name,
                        email = items.Email,
                        dateTime = items.DateTime,
                        status = items.Status,
                        transactionId = items.TransactionId,
                        id = items.Id
                    });
        }

        [HttpGet("sendTransactionMail/{id}")]
        public JsonResult SendTransactionMail(int id)
        {
            try
            {
                BookRequest bookRequest = new BookRequest();
                var transaction = _repository.PaymentTransactionRepository.GetPaymentTransactionById(id);
                bookRequest = JsonConvert.DeserializeObject<BookRequest>(transaction.BookRequest);

                TextPart data;
                StringBuilder sb = new StringBuilder();
                sb.Append("Transaction Detail<br/>");
                sb.Append("Copy and paste the following content to <a href='https://jsonformatter.org/'>https://jsonformatter.org/</a> to format the detail<br/>");
                sb.Append("<br/>");
                sb.Append(JsonConvert.SerializeObject(bookRequest)); ;
                data = new TextPart("html")
                {
                    Text = sb.ToString()
                };

                new EmailHelper().SendMail("developer@adotravel.nl", "Transaction Details", data);
                new EmailHelper().SendMail("cagatay@adotravel.nl", "Transaction Details", data);
                new EmailHelper().SendMail("ekrem@ticketvoordeel.nl", "Transaction Details", data);

                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        #endregion

        #region Views

        [HttpGet("admindashboard")]
        public ActionResult Dashboard()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                TempData["DynamicPagesCount"] = _repository.DynamicPages.GetAllDynamicPages().Count();
                TempData["LastminuteDealsCount"] = _repository.LastMinuteDeals.GetAllLastMinuteDeals().Count();
                TempData["BookingsCount"] = _repository.BookingRepository.GetAllBooking().Count();
                TempData["UsersCount"] = _repository.Profile.GetAllProfiles().Count();
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("managedynamicpages")]
        public ActionResult ManageDynamicpPages()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("managelastminutedeals")]
        public ActionResult ManageLastMinuteDeals()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("manageblogs")]
        public ActionResult ManageBlogs()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("bookings")]
        public ActionResult Bookings()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("usercredits")]
        public ActionResult UserCredits()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("history")]
        public ActionResult TransactionHistory()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("duplicateBookingInfo")]
        public ActionResult DuplicateBookingInfo()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                var bookings = _repository.BookingRepository.GetAllBooking();
                List<string> duplicatesWithName = new List<string>();
                var duplicates = bookings.GroupBy(x => x.DeparturePnr).Where(g => g.Count() > 1);
                List<string> duplicateNames = duplicates.Select(g => g.Key).ToList();

                foreach (var pnr in duplicateNames)
                {
                    if (pnr != "TEST_DEPARTURE")
                    {
                        var bookingDetails = bookings.Where(x => x.DeparturePnr == pnr);
                        if (bookingDetails.Count() > 0)
                        {
                            foreach (var namesItems in bookingDetails)
                            {
                                duplicatesWithName.Add(pnr + "__" + namesItems.CustomerName + "__" + namesItems.CreationTime);
                                break;

                            }
                        }
                    }
                }

                TempData["DuplicateBookingInfo"] = duplicatesWithName;
                return View();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region functions

        public LastMinuteDeals GenerateLastMinuteDeals(LastMinuteDeals lastMinuteDeals, LastMinuteDealsRequest lastMinuteDealsRequest)
        {
            lastMinuteDeals.PoolRequest = new PoolRequest();
            List<Route> routes = new List<Route>();
            Route route = new Route();
            lastMinuteDeals.PoolRequest.Preference = new Preference();
            lastMinuteDeals.CurrencyCode = "EUR";
            lastMinuteDeals.PoolRequest.Preference.CabinType = null;
            lastMinuteDeals.PoolRequest.Preference.RequiredCarrierCodes = null;

            lastMinuteDeals.PoolRequest.Passengers = new Passengers();
            lastMinuteDeals.PoolRequest.Passengers.Adult = 1;
            lastMinuteDeals.PoolRequest.Passengers.Child = 0;
            lastMinuteDeals.PoolRequest.Passengers.Infant = 0;

            route.Origin = new Origin();
            route.Origin.Code = lastMinuteDealsRequest.FromAirportCode;
            route.Origin.IsCity = false;

            route.Destination = new Destination();
            route.Destination.Code = lastMinuteDealsRequest.ToAirportCode;
            route.Destination.IsCity = false;

            route.FlightType = new FlightType();
            route.FlightType.ConnectionType = 2;
            route.FlightType.MaxConnections = null;

            route.Departure = new Departure();
            route.Departure.Date = lastMinuteDealsRequest.TravelDate.Year
                                                                    + "-" + lastMinuteDealsRequest.TravelDate.Month.ToString("MM")
                                                                    + "-" + lastMinuteDealsRequest.TravelDate.Day.ToString("dd");
            route.Departure.DaysAfter = 0;
            route.Departure.DaysBefore = 1;
            routes.Add(route);
            lastMinuteDeals.PoolRequest.Routes = routes;

            return lastMinuteDeals;
        }

        public string SaveFile(IFormFile File, string path)
        {
            try
            {
                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, path);

                if (!System.IO.Directory.Exists(uploads))
                {
                    System.IO.Directory.CreateDirectory(uploads);
                }

                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "");
                GuidString = GuidString.Replace("+", "");
                GuidString = GuidString.Replace("/", "");
                GuidString = GuidString.Replace("\\", "");

                var file = File;
                string imagename = string.Empty;
                if (file != null)
                {
                    string temp = Path.GetFileName(file.FileName);
                    imagename = GuidString + Path.GetExtension(temp);
                    var value = Path.Combine(uploads, GuidString + Path.GetExtension(temp));
                    if (file.Length > 0)
                    {
                        using (var fileStream = new FileStream(value, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                }

                return imagename;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public void SendErrorMail(ErrorMailRequest errorMailRequest)
        {
            TextPart data;
            StringBuilder sbMailContent = new StringBuilder();
            StringBuilder sbFlightData = new StringBuilder();
            sbMailContent.Append("<h3>Ticketvoordeel booking error</h3>");
            if (errorMailRequest.BookRequest.IsReturnAvailable)
            {
                if (errorMailRequest.IsErrorInBoth)
                {
                    sbMailContent.Append("<p><b>Error booking in two way and both of the booking failed</b></p>");
                    sbFlightData.Append("<div>---------------------------------------------</div>");
                    sbFlightData.Append("<div><b>Total Amount: </b>" + errorMailRequest.BookRequest.TotalAmount + "</div>");
                    sbFlightData.Append("<div>Departure Details</div>");
                    sbFlightData.Append("<div>---------------------------------------------</div>");
                    sbFlightData.Append("<div><b>Departure Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Arrival.route.OriginAirport.Code + "</div>");
                    sbFlightData.Append("<div><b>Arrival Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Arrival.route.DestinationAirport.Code + "</div>");
                    sbFlightData.Append("<div><b>Travel Date: </b>" + new DateFormatHelper().FormatDateForMail(errorMailRequest.TravelDate) + "</div>");
                    sbFlightData.Append("<div><b>Amount: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.SalesAmount + "</div>");
                    sbFlightData.Append("<div><b>Basket Key: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.BasketKey + "</div>");
                    sbFlightData.Append("<div>---------------------------------------------</div>");
                    sbFlightData.Append("<div>Return Details</div>");
                    sbFlightData.Append("<div>---------------------------------------------</div>");
                    sbFlightData.Append("<div><b>Departure Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.OriginAirport.Code + "</div>");
                    sbFlightData.Append("<div><b>Arrival Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.DestinationAirport.Code + "</div>");
                    sbFlightData.Append("<div><b>Travel Date: </b>" + new DateFormatHelper().FormatDateForMail(errorMailRequest.TravelDate) + "</div>");
                    sbFlightData.Append("<div><b>Amount: </b>" + errorMailRequest.BookRequest.ReturnBookingObject.SalesAmount + "</div>");
                    sbFlightData.Append("<div><b>Basket Key: </b>" + errorMailRequest.BookRequest.ReturnBookingObject.BasketKey + "</div>");
                }
                else
                {
                    sbMailContent.Append("<p><b>Error booking in two way and one of the booking failed</b></p>");

                    if (errorMailRequest.IsErrorinDeparture)
                    {
                        sbFlightData.Append("<div><b>Departure Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Arrival.route.OriginAirport.Code + "</div>");
                        sbFlightData.Append("<div><b>Arrival Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Arrival.route.DestinationAirport.Code + "</div>");
                        sbFlightData.Append("<div><b>Travel Date: </b>" + new DateFormatHelper().FormatDateForMail(errorMailRequest.TravelDate) + "</div>");
                        sbFlightData.Append("<div><b>Amount: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.SalesAmount + "</div>");
                        sbFlightData.Append("<div><b>Basket Key: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.BasketKey + "</div>");
                    }
                    else
                    {
                        sbFlightData.Append("<div><b>Departure Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.OriginAirport.Code + "</div>");
                        sbFlightData.Append("<div><b>Arrival Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.DestinationAirport.Code + "</div>");
                        sbFlightData.Append("<div><b>Travel Date: </b>" + new DateFormatHelper().FormatDateForMail(errorMailRequest.TravelDate) + "</div>");
                        sbFlightData.Append("<div><b>Amount: </b>" + errorMailRequest.BookRequest.ReturnBookingObject.SalesAmount + "</div>");
                        sbFlightData.Append("<div><b>Basket Key: </b>" + errorMailRequest.BookRequest.ReturnBookingObject.BasketKey + "</div>");
                    }
                }
            }
            else
            {
                if (errorMailRequest.BookRequest.IsMerged)
                {
                    sbMailContent.Append("<p><b>Error booking in a merged booking</b></p>");
                }
                else
                {
                    sbMailContent.Append("<p><b>Error booking in one way</b></p>");
                }
                sbFlightData.Append("<div><b>Departure Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.OriginAirport.Code + "</div>");
                sbFlightData.Append("<div><b>Arrival Airport: </b>" + errorMailRequest.BookRequest.travelInfo.Departure.route.DestinationAirport.Code + "</div>");
                sbFlightData.Append("<div><b>Travel Date: </b>" + new DateFormatHelper().FormatDateForMail(errorMailRequest.TravelDate) + "</div>");
                sbFlightData.Append("<div><b>Amount: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.SalesAmount + "</div>");
                sbFlightData.Append("<div><b>Basket Key: </b>" + errorMailRequest.BookRequest.DepartureBookingObject.BasketKey + "</div>");
            }
            sbMailContent.Append("<div><b>Payment status: </b> Success</div>");
            sbMailContent.Append("<div><b>Name: </b>" + errorMailRequest.BookRequest.MainBooker.FirstName + " " + errorMailRequest.BookRequest.MainBooker.LastName + "</div>");
            sbMailContent.Append("<div><b>User Email: </b>" + errorMailRequest.BookRequest.MainBooker.Email + "</div>");
            sbMailContent.Append("<div><b>Phone: </b>" + errorMailRequest.BookRequest.MainBooker.Phone + "</div>");
            sbMailContent.Append("<div><b>House number: </b>" + errorMailRequest.BookRequest.MainBooker.HouseNumber + "</div>");
            sbMailContent.Append("<div><b>Post code: </b>" + errorMailRequest.BookRequest.MainBooker.PostCode + "</div>");
            sbMailContent.Append("<div><b>Place: </b>" + errorMailRequest.BookRequest.MainBooker.Place + "</div>");
            sbMailContent.Append("<div><b>Street: </b>" + errorMailRequest.BookRequest.MainBooker.Street + "</div>");
            sbMailContent.Append(sbFlightData);
            sbMailContent.Append("<div><b>Passengers: </b></div>");
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            foreach (var passenger in errorMailRequest.BookRequest?.Passengers)
            {
                sbMailContent.Append("<div><b>Title: </b>" + passenger.Title + "</div>");
                sbMailContent.Append("<div><b>FirstName: </b>" + passenger.FirstName + "</div>");
                sbMailContent.Append("<div><b>LastName: </b>" + passenger.LastName + "</div>");
                sbMailContent.Append("<div><b>Birthdate: </b>" + FormatDate(passenger.Birthdate) + "</div>");
            }
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            sbMailContent.Append("<div><b>Flight Details: </b></div>");
            sbMailContent.Append("<div>-----------------------------------------------</div>");

            sbMailContent.Append("<div><b>Departure: </b></div>");
            sbMailContent.Append("<div>" + errorMailRequest.BookRequest?.travelInfo?.Departure?.route?.OriginAirport?.Code
                                + "to" + errorMailRequest.BookRequest?.travelInfo?.Departure?.route?.DestinationAirport?.Code + "</div>");
            foreach (var segment in errorMailRequest.BookRequest?.travelInfo?.Departure?.route?.Segments)
            {
                foreach (var item in segment?.Legs)
                {
                    sbMailContent.Append("<div><b>Origin: </b>" + item?.Origin?.Code + "</div>");
                    sbMailContent.Append("<div><b>Destination: </b>" + item?.Destination?.Code + "</div>");
                    sbMailContent.Append("<div><b>Departure Time: </b>" + item?.DepartureTime.ToString("d/M/yyyy HH:mm:ss") + "</div>");
                    sbMailContent.Append("<div><b>Arrival Time: </b>" + item?.ArrivalTime.ToString("d/M/yyyy HH:mm:ss") + "</div>");
                    sbMailContent.Append("<div><b>Class: </b>" + segment?.BookingClass + "</div>");
                    sbMailContent.Append("<div><b>Flight Number: </b>" + segment.Number + "</div>");
                }
            }
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            if (errorMailRequest.BookRequest?.travelInfo?.Arrival.route != null)
            {
                sbMailContent.Append("<div><b>Return: </b></div>");
                sbMailContent.Append("<div>" + errorMailRequest.BookRequest?.travelInfo?.Arrival?.route?.OriginAirport?.Code
                                    + "to" + errorMailRequest.BookRequest?.travelInfo?.Arrival?.route?.DestinationAirport?.Code + "</div>");
                foreach (var segment in errorMailRequest.BookRequest?.travelInfo?.Arrival?.route?.Segments)
                {
                    foreach (var item in segment?.Legs)
                    {
                        sbMailContent.Append("<div><b>Origin: </b>" + item?.Origin?.Code + "</div>");
                        sbMailContent.Append("<div><b>Destination: </b>" + item?.Destination?.Code + "</div>");
                        sbMailContent.Append("<div><b>Departure Time: </b>" + item?.DepartureTime.ToString("d/M/yyyy HH:mm:ss") + "</div>");
                        sbMailContent.Append("<div><b>Arrival Time: </b>" + item?.ArrivalTime.ToString("d/M/yyyy HH:mm:ss") + "</div>");
                        sbMailContent.Append("<div><b>Class: </b>" + segment?.BookingClass + "</div>");
                        sbMailContent.Append("<div><b>Flight Number: </b>" + segment.Number + "</div>");
                    }
                }
            }

            //External items
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            sbMailContent.Append("<div>External items</div>");
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            if (errorMailRequest.BookRequest?.ExternalItems != null)
            {
                if (errorMailRequest.BookRequest?.ExternalItems.Count() > 0)
                {
                    foreach (var item in errorMailRequest.BookRequest?.ExternalItems)
                    {
                        sbMailContent.Append("<div><b>Name: </b>" + item.Name + "</div>");
                        sbMailContent.Append("<div><b>Amount: </b>" + item.Amount + "</div>");
                    }
                }
            }

            //Discounts
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            sbMailContent.Append("<div>Discounts</div>");
            sbMailContent.Append("<div>-----------------------------------------------</div>");
            if (errorMailRequest.BookRequest?.Discounts != null)
            {
                if (errorMailRequest.BookRequest?.Discounts.Count() > 0)
                {
                    foreach (var item in errorMailRequest.BookRequest?.Discounts)
                    {
                        sbMailContent.Append("<div><b>Name: </b>" + item.Name + "</div>");
                        sbMailContent.Append("<div><b>Amount: </b>" + item.Amount + "</div>");
                    }
                }
            }

            try
            {
                //Booking response
                sbMailContent.Append("<div>-----------------------------------------------</div>");
                sbMailContent.Append("<div>Booking API response</div>");
                sbMailContent.Append("<div>-----------------------------------------------</div>");
                sbMailContent.Append("<div>Departure</div>");
                if (errorMailRequest.BookRequest.DepartureBookingObject != null)
                {
                    sbMailContent.Append("<div>" + JsonConvert.SerializeObject(errorMailRequest.BookRequest.DepartureBookingObject) + "</div>");
                }
                sbMailContent.Append("<div>Return</div>");
                if (errorMailRequest.BookRequest.ReturnBookingObject != null)
                {
                    sbMailContent.Append("<div>" + JsonConvert.SerializeObject(errorMailRequest.BookRequest.ReturnBookingObject) + "</div>");
                }
            }
            catch
            {
            }

            try
            {
                var webRoot = _hostingEnvironment.WebRootPath;
                var PathWithFolderName = Path.Combine(webRoot, "errorlogs");
                Guid guid = Guid.NewGuid();
                string filename = guid.ToString() + ".txt";
                using (StreamWriter writer = System.IO.File.CreateText(PathWithFolderName + "\\" + filename))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(errorMailRequest));
                }
                sbMailContent.Append("<div><b>Send this filename to the developer for more info: </b>" + filename + "</div>");
            }
            catch (Exception ex)
            {
                LogToFile.writeBookingLog("Error mail send error");
            }

            data = new TextPart("html")
            {
                Text = sbMailContent.ToString()
            };

            new EmailHelper().SendErrorMail("info@ticketvoordeel.nl", "Ticketvoordeel booking error", data);
        }

        public string FormatDate(string date)
        {
            return date.Substring(6, 2) + "/" + date.Substring(4, 2) + date.Substring(0, 4);
        }

        #endregion

        #region Mail Feature

        [HttpGet("TicketvoordeelMail")]
        public ActionResult TicketvoordeelMail()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpGet("ticketvoordeelCreateMail")]
        public ActionResult TicketvoordeelCreateMail()
        {
            var bookings = _repository.BookingRepository.GetAllBooking();
            return View();
        }

        [HttpGet("ticketvoordeelHistoryMail")]
        public ActionResult TicketvoordeelHistoryMail()
        {
            var bookings = _repository.BookingRepository.GetAllBooking();
            return View();
        }

        public JsonResult GetUsersForMailing()
        {
            try
            {
                return Json("success");

            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        public string FilterForMailingUsers()
        {
            try
            {
                return "success";

            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public JsonResult TemplateForMailingUsers()
        {
            try
            {
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("f");
            }
        }

        public JsonResult ProcessMail()
        {
            try
            {
                //using (var client = new MailKit.Net.Smtp.SmtpClient())
                //{
                //    client.Connect("smtp.office365.com", 587, false);
                //    client.Authenticate("info@adotravel.nl", "HASTeko2019@gelgit");

                //    var messageContent = new MimeMessage();
                //    messageContent.From.Add(new MailboxAddress("Ticketvoordeel", "info@adotravel.nl"));
                //    messageContent.To.Add(new MailboxAddress("Ticketvoordeel", toAddress));
                //    messageContent.Subject = subject;
                //    messageContent.Body = message;

                //    client.Send(messageContent);
                //    client.Disconnect(true);
                //}
                return Json("success");

            }
            catch (Exception ex)
            {
                return Json("error");
            }
        }

        [HttpGet("tempAutomateBulkEmail")]
        public void TempAutomateBulkEmail()
        {
            List<string> mailUsers = new List<string>();
            List<MailingCreditReport> mailingCreditReportsList = new List<MailingCreditReport>();
            var bookings = _repository.BookingRepository.GetAllBooking();
            var users = _repository.Profile.GetAllProfiles();

            var selection = bookings.Where(c => c.CreditReceived > 500 && c.CreditApplied == 0);
            var i = 0;
            foreach (var userItems in selection)
            {
                var userEmail = users.Where(c => c.Id == userItems.UserId).Select(x => x.Email).FirstOrDefault();
                MailingCreditReport mailingCreditReport = new MailingCreditReport();
                mailingCreditReport.Mail = userEmail;//"noelcmanuel+" + i + "@gmail.com"; //userEmail;
                mailingCreditReport.Name = users.Where(c => c.Id == userItems.UserId).Select(x => x.Name +' '+ x.Surname).FirstOrDefault();
                mailingCreditReport.TotalCredit = userItems.CreditReceived;               
                if(userItems.CreditReceived > 0)
                {
                    if(userItems.CreditReceived >= 500)
                    {
                        var totalTicks = userItems.CreditReceived / 500;
                        if(totalTicks > 0)
                        {
                            var rtotalTicks = Math.Floor(Convert.ToDouble(totalTicks));
                            rtotalTicks = rtotalTicks * 2;
                            mailingCreditReport.CreditInAmount = Convert.ToInt32(rtotalTicks);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                mailingCreditReportsList.Add(mailingCreditReport);
                i++;
            }


            EmailHelper emailHelper = new EmailHelper();
            string subject = "Jouw gespaarde Ticks";


            foreach (var items in mailingCreditReportsList)
            {
                var result = emailHelper.ProcessMailInBulk(subject, _hostingEnvironment, Convert.ToString(items.TotalCredit), Convert.ToString(items.CreditInAmount), items.Name, items.Mail);
                if(result == "true")
                {
                    LogToFile.AdminMailForCredit("True  :"+ result +" "+ items.Name +" "+ items.Mail + " " + Convert.ToString(items.TotalCredit) + " " + Convert.ToString(items.CreditInAmount));
                }
                else
                {
                    LogToFile.AdminMailForCredit("False :" + result + " " + items.Name + " " + items.Mail + " " + Convert.ToString(items.TotalCredit) + " " + Convert.ToString(items.CreditInAmount));
                }
            }
        }

        [HttpGet("subscriptionMail")]
        public ActionResult SubscriptionMail()
        {
            var checkLogin = CheckAdminLoggedIn();
            if (checkLogin == true)
            {
                return View();
            }
            else
            {
                return null;
            }
        }

        [HttpPost("/admin/downloadreport")]
        public string DownloadReport()
        {
            try
            {
                List<string> vs = new List<string>();
                StringBuilder stringBuilder = new StringBuilder();

                var users = _repository.Subscription.GetAllSubscription();               
                foreach (var items in users)
                {
                    vs.Add(items.Email);
                }
                var userList = string.Join(",", vs);
                if(string.IsNullOrEmpty(userList))
                {
                    userList = "List is empty";
                }
                return userList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        [HttpGet("/admin/getsubscription")]
        public JsonResult GetSubscription()
        {
            try
            {
                var users = _repository.Subscription.GetAllSubscription();
                List<Subscription> creditListings = new List<Subscription>();
                Subscription creditListing;
                foreach (var user in users)
                {
                    creditListing = new Subscription();
                    creditListing.Id = user.Id;
                    creditListing.Name = user.Name;
                    creditListing.Email = user.Email;
                    creditListing.IsActive = user.IsActive;
                    creditListing.CreationTime = user.CreationTime;
                    creditListings.Add(creditListing);
                }

                return Json(creditListings);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/admin/deletesubscription")]
        public string DeleteSubscription(int Id)
        {
            try
            {
                Subscription subscription = new Subscription();
                subscription = _repository.Subscription.GetSubscriptionById(Id);
                if(subscription != null)
                {
                    _repository.Subscription.DeleteSubscription(subscription);
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception ex)
            {
                return "false";
            }
        }
        #endregion
    }
}