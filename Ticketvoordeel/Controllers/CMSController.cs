using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.BookRequest;

namespace Ticketvoordeel.Controllers
{
    public class CMSController : Controller
    {
        private IRepositoryWrapper _repository;
        private IWebHostEnvironment _hostingEnvironment;

        public CMSController(IRepositoryWrapper repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _hostingEnvironment = environment;
        }

        [AllowAnonymous]
        [Route("/home/getlastminutedeals")]
        public JsonResult GetLastMinuteDeals()
        {
            try
            {
                CommonFunction commonFunction = new CommonFunction();
             
                List<LastMinuteDealsResponse> lastMinuteDeals = new List<LastMinuteDealsResponse>();
                LastMinuteDealsResponse lastMinuteDealsResponse;
                var lastMinuteDealsList = _repository.LastMinuteDeals.GetAllLastMinuteDeals().ToList<LastMinuteDeal>();
             
                foreach (var items in lastMinuteDealsList)
                {
                    var a = JsonConvert.DeserializeObject(items.PoolSearchRequest);
                    lastMinuteDealsResponse = new LastMinuteDealsResponse();
                    lastMinuteDealsResponse.Id = items.Id;
                    lastMinuteDealsResponse.ArrivalTime = items.ArrivalTime;
                    lastMinuteDealsResponse.ColumnNumber = items.ColumnNumber;
                    lastMinuteDealsResponse.DepartureTime = items.DepartureTime;
                    lastMinuteDealsResponse.FromLocation = items.FromLocation;
                    lastMinuteDealsResponse.IsActive = items.IsActive;
                    lastMinuteDealsResponse.Price = items.Price;
                    lastMinuteDealsResponse.Title = items.Title;
                    lastMinuteDealsResponse.ToLocation = items.ToLocation;
                    lastMinuteDealsResponse.Airline = items.Airline;
                    lastMinuteDealsResponse.PoolRequest = JsonConvert.DeserializeObject<LastMinuteDeals>(items.PoolSearchRequest);
                    lastMinuteDeals.Add(lastMinuteDealsResponse);
                }

                return Json(new Response { Data = lastMinuteDeals, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [AllowAnonymous]
        [Route("home/updatelastminutedeals")]
        public void GetUpdateLastMinuteDeals()
        {
            try
            {
                CommonFunction commonFunction = new CommonFunction();
                List<LastMinuteDealsResponse> lastMinuteDeals = new List<LastMinuteDealsResponse>();
                var lastMinuteDealsList = _repository.LastMinuteDeals.GetAllLastMinuteDeals().ToList<LastMinuteDeal>();
                commonFunction.FetchAndStoreLastMinuteDetail(lastMinuteDealsList, _repository);                
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
            }
        }

        [HttpPost]
        [Route("sendfaqrequest")]
        public JsonResult SendFaqRequest([FromBody]FaqRequest faqRequest)
        {
            try
            {
                new EmailHelper().SendFaqRequest("info@ticketvoordeel.nl", "FAQ Request", faqRequest);
                return Json(new Response { Data = String.Empty, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost]
        [Route("sendcontactrequest")]
        public JsonResult SendContactRequest([FromBody]ContactRequest contactRequest)
        {
            try
            {
                TextPart data;
                string mailContent = "<h3>Contact Request</h3>";
                mailContent += "<div><b>Salutation : </b>" + contactRequest.Salutation + "</div><div><b>First Name : </b>" + contactRequest.FirstName + "</div><div><b>Last Name : </b>" + contactRequest.LastName + "</div>";
                mailContent += "<div><b>Phone : </b>" + contactRequest.Phone + "</div><div><b>Email : </b>" + contactRequest.Email + "</div><div><b>Booking Number : </b>" + contactRequest.BookingNumber + "</div>";
                mailContent += "<div><b>Description : </b>" + contactRequest.Description + "</div>";
                data = new TextPart("html")
                {
                    Text = mailContent
                };

                new EmailHelper().SendMail("info@ticketvoordeel.nl", "Contact Request", data);

                _repository.ContactRequests.CreateContactRequest(contactRequest);

                return Json(new Response { Data = String.Empty, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("blog/{name}")]
        public JsonResult Blog(string name)
        {
            try
            {
                DynamicPage dynamicPage = new DynamicPage();
                dynamicPage = _repository.DynamicPages.GetDynamicPages(c => c.Name == name).FirstOrDefault();
                return Json(new Response { Data = dynamicPage, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getuploadcategories")]
        public JsonResult GetUploadCategory()
        {
            try
            {                
                return Json(new Response { Data = _repository.UploadCategories.GetAllUploadCategories(), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost]
        [Route("uploadimages")]
        public JsonResult UploadImages(int categoryId, List<IFormFile> files)
        {
            try
            {
                if (files != null & files.Count > 0)
                {
                    List<ImageRepository> imageRepositories = new List<ImageRepository>();
                    ImageRepository imageRepository;
                    foreach (var file in files)
                    {
                        //string fileName = new S3UploadHelper().UploadObject(file, "banner").Result;
                        string fileName = "error";
                        if (fileName != "error")
                        {
                            imageRepository = new ImageRepository();
                            imageRepository.CategoryId = categoryId;
                            imageRepository.FileName = fileName;
                            imageRepository.OriginalName = file.FileName;
                            imageRepository.Path = "banner" + "/" + fileName;
                            imageRepository.FullPath = Utils.Constants.S3Url + "/" + "banner" + "/" + fileName;
                            _repository.ImageRepository.CreateImageRepository(imageRepository);
                        }
                    }
                }
                return Json(new Response { Data = _repository.ImageRepository.GetImageRepository(c => c.CategoryId == categoryId), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("deleteimage/{id}")]
        public JsonResult DeleteImage(int id)
        {
            try
            {
                ImageRepository imageRepository = new ImageRepository();
                imageRepository = _repository.ImageRepository.GetImageRepositoryById(id);
                //bool status = new S3UploadHelper().RemoveObject(imageRepository.Path).Result;
                bool status = false;
                if (status)
                {
                    _repository.ImageRepository.DeleteImageRepository(imageRepository);
                    return Json(new Response { Data = String.Empty, Message = "success", Status = true });
                }
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getimages/{id}")]
        public JsonResult GetImages(int id)
        {
            try
            {                
                return Json(new Response { Data = _repository.ImageRepository.GetImageRepository(c => c.CategoryId == id), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("changeBlogStatus/{id}/{status}")]
        public JsonResult ChangeBlogStatus(int id, bool status)
        {
            try
            {
                Blog blog = new Blog();
                blog = _repository.BlogRepository.GetBlogById(id);
                blog.IsActive = status;
                _repository.BlogRepository.UpdateBlog(blog);
                blog = _repository.BlogRepository.GetBlogById(id);
                if(blog.IsActive == status)
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
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getbanner/{id}")]
        public JsonResult GetBanner(int id)
        {
            try
            {
                return Json(new Response { Data = _repository.BannerRepository.GetBannerById(id), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getbanners")]
        public JsonResult GetBanners()
        {
            try
            {
                return Json(new Response { Data = _repository.BannerRepository.GetAllBanner(), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("deletebanner/{id}")]
        public JsonResult DeleteBanner(int id)
        {
            try
            {
                Banner banner = _repository.BannerRepository.GetBannerById(id);
                return Json(new Response { Data = _repository.BannerRepository.DeleteBanner(banner), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost]
        [Route("uploadbanner")]
        [AllowAnonymous]
        public JsonResult UploadBAnner(Banner banner, List<IFormFile> files)
        {
            try
            {
                if (files != null & files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        //string fileName = new S3UploadHelper().UploadObject(file, "banner").Result;
                        string fileName = "error";
                        if (fileName != "error")
                        {
                            banner.Image = "banner" + "/" + fileName;
                        }
                    }
                }
                if (banner.Id > 0)
                {
                    _repository.BannerRepository.UpdateBanner(banner);
                }
                else
                {
                    _repository.BannerRepository.CreateBanner(banner);
                }
                if (banner.Id > 0)
                {
                    return Json(new Response { Data = banner, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = banner, Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getdynamicpages")]
        [AllowAnonymous]
        public JsonResult GetDynamicPages()
        {
            try
            {
                var query = _repository.DynamicPages.GetAllDynamicPages();
                var result = from items in query
                             select new
                             {
                                 Id = items.Id,
                                 URL = items.Name,
                                 Title = items.Title,
                                 Type = items.Type
                             };

                return Json(new Response { Data = result, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet]
        [Route("getdynamicpage/{id}")]
        [AllowAnonymous]
        public JsonResult GetDynamicPage(string id)
        {
            try
            {
                return Json(new Response { Data = _repository.DynamicPages.GetDynamicPages(c => c.Name == id), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [HttpGet("/getblogs")]
        public JsonResult GetBlogs()
        {
            try
            {
                var query = from items in _repository.DynamicPages.GetAllDynamicPages().Where(c => c.Type == "Blog").Take(4)
                            select new
                            {
                                Id = items.Id,
                                Title = items.Title,
                                Link = items.Name,
                                Type = items.Type,
                                FromAirport = items.FromAirport,
                                ToAirport = items.ToAirport,
                                Image = items.Image,
                                ShortDescription = items.ShortDescription
                            };
                return Json(query);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/getallblogs")]
        public JsonResult GetAllBlogs()
        {
            try
            {
                var query = from items in _repository.DynamicPages.GetAllDynamicPages().Where(c => c.Type == "Blog")
                            select new
                            {
                                Id = items.Id,
                                Title = items.Title,
                                Link = items.Name,
                                Type = items.Type,
                                FromAirport = items.FromAirport,
                                ToAirport = items.ToAirport,
                                Image = items.Image,
                                ShortDescription = items.ShortDescription
                            };
                return Json(query);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpGet("/getfooterlinks")]
        public JsonResult GetFooterLinks()
        {
            try
            {
                var query = from items in _repository.DynamicPages.GetAllDynamicPages().Where(c => c.Type == "Footer")
                            select new
                            {
                                Id = items.Id,
                                Title = items.Title,
                                Link = items.Name
                            };
                return Json(query);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpPost]
        [Route("incompletebookingmail")]
        [AllowAnonymous]
        public async void IncompleteBookingMail([FromBody] IncompleteBookingDetails incompleteBookingDetails)
        {
            try
            {
                if(incompleteBookingDetails != null && incompleteBookingDetails.step != "error")
                {
                    string img = incompleteBookingDetails.imageData;
                    if (img != null)
                    {
                        img = img.Replace("data:image/png;base64,", "");
                        byte[] imageBytes = Convert.FromBase64String(img);
                        var id = incompleteBookingDetails.uniqueId;
                        var isSaving = true;
                        string folderPath = _hostingEnvironment.WebRootPath + "/uploads/IncompleteBooking/" + id;
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        IncompleteBooking incompleteBooking = new IncompleteBooking();

                        var previousRecord = _repository.IncompleteBooking.GetAllIncompleteBooking();
                        if(previousRecord != null && previousRecord.Count() > 0)
                        {
                            var specficRecord = previousRecord.Where(c => c.UniqueId == incompleteBookingDetails.uniqueId);
                            if(specficRecord != null && specficRecord.Count() > 0)
                            {
                                incompleteBooking = specficRecord.FirstOrDefault();
                            }
                            else
                            {
                                incompleteBooking.CreationTime = DateTime.Now;
                                incompleteBooking.UniqueId = incompleteBookingDetails.uniqueId;
                                incompleteBooking.IsMailSent = false;
                            }
                        }
                        else
                        {
                            incompleteBooking.CreationTime = DateTime.Now;
                            incompleteBooking.UniqueId = incompleteBookingDetails.uniqueId;
                            incompleteBooking.IsMailSent = false;
                        }

                        

                        if (!string.IsNullOrEmpty(incompleteBookingDetails.step) && incompleteBookingDetails.step == "stepone")
                        {
                            if (!string.IsNullOrEmpty(incompleteBookingDetails.personalDetails))
                            {
                                var incompleteBookingParams = JsonConvert.DeserializeObject<IncompleteBookingParams>(incompleteBookingDetails.personalDetails);
                                incompleteBooking.personalDetails = JsonConvert.SerializeObject(incompleteBookingParams);
                                incompleteBooking.Name = incompleteBookingParams.contactDetails.FirstName;
                                if (string.IsNullOrEmpty(incompleteBooking.Name))
                                {
                                    isSaving = false;
                                }
                                incompleteBooking.Email = incompleteBookingParams.contactDetails.email;
                                if (string.IsNullOrEmpty(incompleteBooking.Email))
                                {
                                    isSaving = false;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(incompleteBookingDetails.FirstName))
                                {
                                    incompleteBooking.Name = incompleteBookingDetails.FirstName;
                                }
                                if (!string.IsNullOrEmpty(incompleteBookingDetails.FirstEmail))
                                {
                                    incompleteBooking.Email = incompleteBookingDetails.FirstEmail;
                                }
                            }
                            var path = folderPath + "/" + "1.png";
                            incompleteBooking.steponepath = path;
                            if (!string.IsNullOrEmpty(path))
                            {
                                System.IO.File.WriteAllBytes(path, imageBytes);
                            }
                            else
                            {
                                isSaving = false;
                            }
                        }
                        else if (!string.IsNullOrEmpty(incompleteBookingDetails.step) && incompleteBookingDetails.step == "steptwo")
                        {
                            var path = folderPath + "/" + "2.png";
                            incompleteBooking.steptwopath = path;
                            if (!string.IsNullOrEmpty(path))
                            {
                                System.IO.File.WriteAllBytes(path, imageBytes);
                            }
                            else
                            {
                                isSaving = false;
                            }
                        }
                        else if (!string.IsNullOrEmpty(incompleteBookingDetails.step) && incompleteBookingDetails.step == "stepthree")
                        {
                            var path = folderPath + "/" + "4.png";
                            incompleteBooking.stepthreepath = path;
                            if (!string.IsNullOrEmpty(path))
                            {
                                System.IO.File.WriteAllBytes(path, imageBytes);
                            }
                            else
                            {
                                isSaving = false;
                            }
                        }
                        else if (!string.IsNullOrEmpty(incompleteBookingDetails.step) && incompleteBookingDetails.step == "stepfour")
                        {
                            var path = folderPath + "/" + "3.png";
                            incompleteBooking.stepfourpath = path;
                            if (!string.IsNullOrEmpty(path))
                            {
                                System.IO.File.WriteAllBytes(path, imageBytes);
                            }
                            else
                            {
                                isSaving = false;
                            }
                        }

                        if (incompleteBooking.Id > 0)
                        {
                            _repository.IncompleteBooking.UpdateIncompleteBooking(incompleteBooking);
                        }
                        else
                        {
                            _repository.IncompleteBooking.CreateIncompleteBooking(incompleteBooking);
                        }

                    }
                }
                else
                {
                    IncompleteBooking incompleteBooking = new IncompleteBooking();
                    var previousRecordForDelete = _repository.IncompleteBooking.GetAllIncompleteBooking();
                    if (previousRecordForDelete != null && previousRecordForDelete.Count() > 0)
                    {
                        var specficRecordForDelete = previousRecordForDelete.Where(c => c.UniqueId == incompleteBookingDetails.uniqueId);
                        if (specficRecordForDelete != null && specficRecordForDelete.Count() > 0)
                        {
                            incompleteBooking = specficRecordForDelete.FirstOrDefault();
                            _repository.IncompleteBooking.DeleteIncompleteBooking(incompleteBooking);
                        }                        
                    }
                }
             
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
            }
        }


        [HttpPost]
        [Route("incompletebookingsentmail")]
        [AllowAnonymous]
        public JsonResult IncompleteBookingSentMail()
        {
            try
            {
                List<int> deleteList = new List<int>();
                var getAllDetails = _repository.IncompleteBooking.GetAllIncompleteBooking();
                if(getAllDetails != null && getAllDetails.Count() > 0)
                {
                    foreach(var items in getAllDetails)
                    {
                        DateTime givenDateTime = items.CreationTime;
                        TimeSpan timeElapsed = DateTime.Now - givenDateTime;

                        if(timeElapsed.TotalMinutes > 20 && !string.IsNullOrEmpty(items.UniqueId) && !string.IsNullOrEmpty(items.Name) && !string.IsNullOrEmpty(items.Email) && items.IsMailSent == false)
                        {
                            LogToFile.RecordIncompleteBooking(JsonConvert.SerializeObject(items));
                            EmailHelper emailHelper = new EmailHelper();
                            deleteList.Add(items.Id);
                            emailHelper.ProcessMailForIncompleteBooking(_hostingEnvironment, items.Name, items.Email, "user", items);
                            emailHelper.ProcessMailForIncompleteBooking(_hostingEnvironment, "Ekrem", "claudious.neol@polussoftware.com", "admin", items);
                            emailHelper.ProcessMailForIncompleteBooking(_hostingEnvironment, "Ekrem", "info@ticketvoordeel.nl", "admin", items);

                        }
                    }

                    if (deleteList != null)
                    {
                        foreach (var item in deleteList)
                        {
                            var getDetails = _repository.IncompleteBooking.GetIncompleteBookingById(item);
                            if (getDetails != null)
                            {
                                _repository.IncompleteBooking.DeleteIncompleteBooking(getDetails);
                                DeleteFolderIncompleteBooking(getDetails.UniqueId);
                            }
                        }
                    }

                    return Json(new Response { Data = String.Empty, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = "Empty Records", Message = "NotFound", Status = true });

                }
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        public ActionResult DeleteFolderIncompleteBooking(string id)
        {
            try
            {
                string folderPath = _hostingEnvironment.WebRootPath + "/Uploads/IncompleteBooking/" + id.ToString();
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                return Json("true");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("sentsunexpressremaindermail")]
        [AllowAnonymous]
        public string SentSunExpressRemainderMail()
        {
            try
            {
                var bookingDetails = _repository.BookingRepository.GetAllBooking();
                if(bookingDetails != null && bookingDetails.Count() > 0)
                {
                    bookingDetails = bookingDetails.Where(x => x.CreationTime.Year > 2021);
                    if(bookingDetails.Count() > 0)
                    {
                        foreach(var items in bookingDetails)
                        {
                            var isSunExpressMail = false;

                            var departureDetails = JsonConvert.DeserializeObject<BookResponse>(items.BookingDetails);
                            if (departureDetails != null)
                            {
                                if (items.IsRoundTrip == false && departureDetails.DepartureBookResponse.HasProblem == false)
                                {
                                    if (departureDetails.Request.travelInfo.Departure.route.Segments[0].Carrier.Name.ToLower() == "sunexpress")
                                    {
                                        var departureDate = departureDetails.Request.travelInfo.Departure.route.Segments[0].Legs[0].DepartureTime;
                                        if(departureDate != null && departureDate.Date.AddDays(-3) == DateTime.Now.Date)
                                        {
                                            isSunExpressMail = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (departureDetails.ReturnBookResponse.HasProblem == false && departureDetails.DepartureBookResponse.HasProblem == false)
                                    {
                                        if (departureDetails.Request.travelInfo.Departure.route.Segments[0].Carrier.Name.ToLower() == "sunexpress")
                                        {
                                            var departureDate = departureDetails.Request.travelInfo.Departure.route.Segments[0].Legs[0].DepartureTime;
                                            if (departureDate != null && departureDate.Date.AddDays(-3) == DateTime.Now.Date)
                                            {
                                                isSunExpressMail = true;
                                            }
                                        }

                                        if (departureDetails.Request.travelInfo.Arrival.route.Segments[0].Carrier.Name.ToLower() == "sunexpress" && !isSunExpressMail)
                                        {
                                            var arrivaleDate = departureDetails.Request.travelInfo.Arrival.route.Segments[0].Legs[0].DepartureTime;
                                            if (arrivaleDate != null && arrivaleDate.Date.AddDays(-3) == DateTime.Now.Date)
                                            {
                                                isSunExpressMail = true;
                                            }
                                        }
                                    }
                                }
                            }


                            if(isSunExpressMail)
                            {
                                LogToFile.RecordSunExpressRemainderMail(items.Id +"  "+  departureDetails.Request.MainBooker.FirstName + "DeparturePnr:" + items.DeparturePnr + " ReturnPnr:" + items.ReturnPnr); ;
                            }

                        }
                        
                    }
                }
                return null;
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("getlocationoffset")]
        public async Task<int> GetLocationOffset(string depLocation, string arrLocation)
        {
            string baseUrl = "http://api.timezonedb.com/v2.1/convert-time-zone?key=EKSHN213XFN5&format=json&from=";
            string apiUrl = baseUrl + depLocation + "&to=" + arrLocation;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://api.timezonedb.com/v2.1/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TimeZoneLocation>(responseBody);
                    if (jsonResponse.status == "OK")
                    {
                        return Convert.ToInt32(jsonResponse.offset);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}