using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using Ticketvoordeel.Entities;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.BookRequest;
using Ticketvoordeel.Services;

namespace Ticketvoordeel.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private IUserService _userService;
        private IRepositoryWrapper _repository;

        public UserController(IRepositoryWrapper repository, IUserService userService)
        {
            _userService = userService;
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost("/users/authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Route("/user/create")]
        [HttpPost]
        public JsonResult Create([FromBody]Profile profile)
        {
            try
            {
                Profile oldProfile = new Profile();
                oldProfile = _repository.Profile.GetProfiles(c => profile.Email.ToLower() == c.Email.ToLower()).FirstOrDefault();

                if (new EmailHelper().IsValid(profile.Email))
                {
                    if (profile.Password.Length < 6)
                    {
                        return Json(new Response { Data = String.Empty, Message = "Password should have atleast 6 characters", Status = false });
                    }
                    else
                    {
                        if (oldProfile?.Id > 0)
                        {
                            return Json(new Response { Data = String.Empty, Message = "Email already exist", Status = false });
                        }
                        else
                        {
                            oldProfile = _repository.Profile.GetProfiles(c => profile.Mobile == c.Mobile).FirstOrDefault();

                            if (oldProfile?.Id > 0)
                            {
                                return Json(new Response { Data = String.Empty, Message = "Mobile number already exist", Status = false });
                            }
                            else
                            {
                                var returnProfile = _repository.Profile.CreateProfile(profile);
                                returnProfile.Password = "******";
                                return Json(new Response { Data = returnProfile, Message = "success", Status = true });
                            }
                        }
                    }
                }
                else
                {
                    return Json(new Response { Data = profile, Message = "Invalid email", Status = false });
                }
            }
            catch (Exception e)
            {
                return Json(new Response { Data = profile, Message = "Error", Status = false });
            }
        }

        [Route("/user/login")]
        [HttpPost]
        public JsonResult Userlogin([FromBody]Profile profile)
        {
            try
            {
                Profile oldProfile = new Profile();
                oldProfile = _repository.Profile.GetProfiles(c => (c.Email == profile.Email && c.Password == profile.Password)).FirstOrDefault();
                profile.Password = String.Empty;
                if (oldProfile != null && oldProfile.Id > 0)
                {
                    oldProfile.Password = "******";
                    return Json(new Response { Data = oldProfile, Message = "success", Status = true });
                }
                else
                {
                    profile.Password = "******";
                    return Json(new Response { Data = profile, Message = "account does not exist", Status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = String.Empty, Message = "Error", Status = false });
            }
        }

        [Route("/user/get/{id}")]
        [HttpGet]
        public JsonResult GetUser(int id)
        {
            try
            {
                Profile profile = new Profile();
                profile = _repository.Profile.GetProfiles(c => c.Id == id).FirstOrDefault();
                profile.Password = String.Empty;
                return Json(new Response { Data = profile, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "Error", Status = false });
            }
        }

        [Route("/user/getbookings")]
        [HttpPost]
        public JsonResult GetBookings([FromBody]Profile profile)
        {
            try
            {
                var user = _repository.Profile.GetProfiles(c => c.Email == profile.Email).FirstOrDefault();
                var bookings = _repository.BookingRepository.GetBooking(c => c.UserId == user.Id);
                return Json(new Response { Data = bookings, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "Error", Status = false });
            }
        }

        [Route("/user/getbookingDetail")]
        [HttpPost]
        public JsonResult GetBookingDetail([FromBody]Booking booking)
        {
            try
            {
                var bookingData = _repository.BookingRepository.GetBookingById(booking.Id);
                BookResponse bookResponse = new BookResponse();
                bookResponse = JsonConvert.DeserializeObject<BookResponse>(bookingData.BookingDetails);
                return Json(new Response { Data = bookResponse, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "Error", Status = false });
            }
        }

        [Route("/user/getcredits")]
        [HttpPost]
        public JsonResult GetCredits([FromBody]Profile profile)
        {
            try
            {
                var user = _repository.Profile.GetProfiles(c => c.Email == profile.Email).FirstOrDefault();
                List<CreditViewModel> creditViewModels = new List<CreditViewModel>();
                CreditViewModel creditViewModel;
                int totalCredit = 0;
                var bookings = _repository.BookingRepository.GetBooking(c => c.UserId == user.Id);
                foreach (var booking in bookings)
                {
                    totalCredit += booking.CreditReceived;
                    totalCredit -= booking.CreditApplied;
                    creditViewModel = new CreditViewModel();
                    creditViewModel.AvailableCredits = totalCredit.ToString();
                    creditViewModel.CreatedDate = booking.CreationTime;
                    creditViewModel.Credits = booking.CreditReceived.ToString();
                    creditViewModel.Description = booking.CreditReceived.ToString()
                        + " Ticks ontvangen and " + booking.CreditApplied.ToString() + " Ticks ingewisseld";
                    creditViewModels.Add(creditViewModel);
                }

                return Json(new Response { Data = creditViewModels, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "Error", Status = false });
            }
        }

        [Route("/user/getcreditcoupon")]
        [HttpPost]
        public JsonResult GetCreditCoupon([FromBody]Profile profile)
        {
            try
            {
                var user = _repository.Profile.GetProfiles(c => c.Email == profile.Email).FirstOrDefault();
                CreditCouponViewModel creditCouponViewModel = new CreditCouponViewModel();
                var bookings = _repository.BookingRepository.GetBooking(c => c.UserId == user.Id);
                int totalCredited = 0, totalApplied = 0;
                foreach (var booking in bookings)
                {
                    totalCredited += booking.CreditReceived;
                    totalApplied += booking.CreditApplied;
                }
                creditCouponViewModel.TotalCouponAvailable = totalCredited - totalApplied;
                creditCouponViewModel.TotalCouponsApplicable = creditCouponViewModel.TotalCouponAvailable / 500;
                creditCouponViewModel.AmountToDeduct = creditCouponViewModel.TotalCouponsApplicable * 2;
                creditCouponViewModel.TotalCouponsApplicable *= 500;

                return Json(new Response { Data = creditCouponViewModel, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "Error", Status = false });
            }
        }

        [Route("/user/changepassword")]
        [HttpPost]
        public JsonResult ChangePassword([FromBody] Profile profile)
        {
            try
            {
                Profile existingUser = new Profile();
                existingUser = _repository.Profile.GetProfiles(c => c.Id == profile.Id).FirstOrDefault();
                existingUser.Password = profile.Password;
                _repository.Profile.UpdateProfile(existingUser);
                existingUser.Password = String.Empty;
                return Json(new Response { Data = existingUser, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "error", Status = false });
            }
        }

        [Route("/user/update")]
        [HttpPost]
        public JsonResult UpdateUser([FromBody] Profile profile)
        {
            try
            {
                Profile existingUser = new Profile();
                existingUser = _repository.Profile.GetProfiles(c => c.Email == profile.Email).FirstOrDefault();
                existingUser.DOB = profile.DOB;
                existingUser.Mobile = profile.Mobile;
                existingUser.Name = profile.Name;
                existingUser.Surname = profile.Surname;
                if (!string.IsNullOrEmpty(profile.Password))
                {
                    existingUser.Password = profile.Password;
                }               
                _repository.Profile.UpdateProfile(existingUser);
                existingUser.Password = "******";
                return Json(new Response { Data = existingUser, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = "", Message = "error", Status = false });
            }
        }

        [Route("/generateforgotpassword")]
        [HttpPost]
        public JsonResult Generateforgotpassword([FromBody] ResetPassword resetPassword)
        {
            try
            {
                var profile = _repository.Profile.GetProfiles(c => c.Email == resetPassword.Email).FirstOrDefault();
                Guid obj = Guid.NewGuid();
                profile.ResetCode = obj.ToString();
                _repository.Profile.UpdateProfile(profile);

                TextPart data;
                string mailContent = "<div><b>Beste " + profile.Name + "</b></div>";
                mailContent += "<div><b>We hebben uw verzoek ontvangen om uw wachtwoord te resetten.</b></div>";
                mailContent += "<div><b>Klik s.v.p.op de onderstaande link om uw wachtwoord te wijzigen:</b></div><br>";
                mailContent += "<div><a href='https://ticketvoordeel.nl/resetpassword/" + profile.ResetCode + "'>Click to reset</a></div><br>";
                mailContent += "<div><b>Met vriendelijke groet</b></div>";
                mailContent += "<div><b>Team Ticketvoordeel.nl</b></div>";


                data = new TextPart("html")
                {
                    Text = mailContent
                };

                new EmailHelper().SendMail(resetPassword.Email, "Reset Password", data);

                return Json(new Response { Data = "", Message = "success", Status = true });
            }
            catch(Exception ex)
            {
                LogToFile.writeLog(ex.ToString());
                return Json(new Response { Data = "", Message = "error", Status = false });
            }
        }

        [Route("/updatepassword")]
        [HttpPost]
        public JsonResult Updatepassword([FromBody] UpdatePassword updatePassword)
        {
            try
            {
                if (!String.IsNullOrEmpty(updatePassword.Code))
                {
                    var profile = _repository.Profile.GetProfiles(c => c.ResetCode == updatePassword.Code).FirstOrDefault();
                    profile.ResetCode = String.Empty;
                    profile.Password = updatePassword.Password;
                    _repository.Profile.UpdateProfile(profile);

                    return Json(new Response { Data = "", Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = "", Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.ToString());
                return Json(new Response { Data = "", Message = "error", Status = false });
            }
        }
    }
}