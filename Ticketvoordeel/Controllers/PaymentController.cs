using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.BookRequest;
using Ticketvoordeel.Payment;

namespace Ticketvoordeel.Controllers
{
    public class PaymentController : Controller
    {
        private IRepositoryWrapper _repository;

        public PaymentController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [Route("/payment/getbankslist")]
        [AllowAnonymous]
        public async Task<JsonResult> GetBanksListAsync()
        {
            try
            {
                SisowPaymentGateway sisowPaymentGateway = new SisowPaymentGateway();
                List<SisowIssuer> sisowIssuerList = new List<SisowIssuer>();
                sisowIssuerList = await sisowPaymentGateway.DirectoryRequestAsync();
                return Json(new Response { Data = sisowIssuerList, Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        [HttpPost("/payment/getpaymenturl")]
        [AllowAnonymous]
        public async Task<JsonResult> GotoPayment([FromBody]PaymentURLRequest paymentURLRequest)
        {
            try
            {
                LogToFile.PaymentUrl(JsonConvert.SerializeObject(paymentURLRequest), "1");
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(paymentURLRequest), "1");
                RegisterUser(paymentURLRequest.bookRequest);
                SisowPaymentGateway sisowPaymentGateway = new SisowPaymentGateway();
                sisowPaymentGateway.amount = paymentURLRequest.amount;
                if (paymentURLRequest.paymentType == "paypalec")
                {
                    sisowPaymentGateway.entranceCode = "paypalec";
                    sisowPaymentGateway.amount += Math.Round((sisowPaymentGateway.amount * 3.4) / 100, 2);
                }
                if (paymentURLRequest.paymentType.ToLower() == "bancontact")
                {
                    //sisowPaymentGateway.entranceCode = "bancontact";
                    //sisowPaymentGateway.amount += Math.Round((sisowPaymentGateway.amount * 2.5) / 100, 2);
                    sisowPaymentGateway.amount += 0.49;
                }
                if (paymentURLRequest.paymentType.ToLower() == "sofort")
                {
                    //sisowPaymentGateway.entranceCode = "sofort";
                    sisowPaymentGateway.amount += Math.Round((sisowPaymentGateway.amount * 1) / 100, 2);
                }
                if (paymentURLRequest.paymentType == "IDEAL")
                {
                    sisowPaymentGateway.issuerId = paymentURLRequest.issuerId;
                }
                else
                {
                    sisowPaymentGateway.payment = paymentURLRequest.paymentType;
                }
                sisowPaymentGateway.returnUrl = "https://ticketvoordeel.nl/vliegtickets/payment/success";
                sisowPaymentGateway.callbackUrl = "https://ticketvoordeel.nl/vliegtickets/payment/success";
                sisowPaymentGateway.notifyUrl = "https://ticketvoordeel.nl/vliegtickets/payment/success";
                sisowPaymentGateway.cancelUrl = "https://ticketvoordeel.nl/vliegtickets/payment/error";
                sisowPaymentGateway.purchaseId = new EncryptionHelper().Create16DigitString();
                sisowPaymentGateway.description = paymentURLRequest.bookRequest.UserEmail;
                sisowPaymentGateway.testMode = false;

                LogToFile.PaymentUrl(JsonConvert.SerializeObject(sisowPaymentGateway), "2");
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(sisowPaymentGateway), "2");


                await sisowPaymentGateway.TransactionRequest();

                PaymentTransaction paymentTransaction = new PaymentTransaction();
                paymentTransaction.TransactionId = sisowPaymentGateway.trxId;
                paymentTransaction.BookRequest = JsonConvert.SerializeObject(paymentURLRequest.bookRequest);
                paymentTransaction.Email = paymentURLRequest.bookRequest.UserEmail;
                paymentTransaction.Name = paymentURLRequest.bookRequest.MainBooker.FirstName + " " + paymentURLRequest.bookRequest.MainBooker.LastName;
                paymentTransaction.Status = "Pending";
                paymentTransaction.Amount = paymentURLRequest.amount.ToString();
                paymentTransaction.DateTime = GetCurrentTimeAsString();

                LogToFile.PaymentUrl(JsonConvert.SerializeObject(paymentTransaction), "3");
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(paymentTransaction), "3");


                _repository.PaymentTransactionRepository.CreatePaymentTransaction(paymentTransaction);

                if (paymentTransaction.Id == 0)
                {
                    LogToFile.writePaymentLog("Error saving payment transaction" + JsonConvert.SerializeObject(paymentTransaction));
                    LogToFile.SpecficPaymentUrl("Error saving payment transaction" + JsonConvert.SerializeObject(paymentTransaction), "3+");

                }

                PaymentURLResponse paymentURLResponse = new PaymentURLResponse();
                paymentURLResponse.PaymentURL = sisowPaymentGateway.issuerUrl;
                paymentURLResponse.TransactionId = sisowPaymentGateway.trxId;
                LogToFile.PaymentUrl(JsonConvert.SerializeObject(paymentURLResponse), "4");
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(paymentURLResponse), "4");

                return Json(new Response { Data = paymentURLResponse, Message = "success", Status = true });
            }
            catch(Exception ex)
            {
                LogToFile.PaymentUrl(JsonConvert.SerializeObject(ex), "5");
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(ex), "5");

                LogToFile.writePaymentLog("/payment/getpaymenturl" + ex.Message);
                LogToFile.writePaymentLog("/payment/getpaymenturl" + ex.InnerException);
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        #region "User"

        public void RegisterUser(BookRequest bookRequest)
        {
            try
            {
                var user = _repository.Profile.GetProfiles(c => c.Email == bookRequest.UserEmail).FirstOrDefault();
                Profile profile = new Profile();
                if (user == null || user.Id <= 0)
                {
                    profile.Email = bookRequest.UserEmail;
                    profile.DOB = DateTime.Parse(bookRequest.MainBooker.BirthDate.Substring(0, 4)
                                    + "-" + bookRequest.MainBooker.BirthDate.Substring(4, 2)
                                    + "-" + bookRequest.MainBooker.BirthDate.Substring(6, 2));
                    profile.IsActive = true;
                    profile.IsVerified = true;
                    profile.Mobile = bookRequest.MainBooker.Phone;
                    profile.Name = bookRequest.Passengers[0].FirstName;
                    profile.Password = RandomPassword();
                    profile.Surname = bookRequest.Passengers[0].LastName;
                    _repository.Profile.CreateProfile(profile);
                    try
                    {
                        TextPart data;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<h3>Welkom als lid bij Ticketvoordeel.nl</h3>");
                        sb.Append("<div>Beste " + profile.Name + ",</div>");
                        sb.Append("<div>Allereerst bedankt voor uw leuke boeking.</div>");
                        sb.Append("<div>Hieronder treft u uw gebruikersnaam en wachtwoord aan voor uw Ticketvoordeel.nl account.</div>");
                        sb.Append("<div>U kunt met de onderstaande gegevens inloggen via 'Mijn Boeking' via www.ticketvoordeel.nl</div>");
                        sb.Append("<div>Uw gebruikersnaam : " + profile.Email + "</div>");
                        sb.Append("<div>Uw wachtwoord : " + profile.Password + "</div>");
                        sb.Append("<div>U kunt inloggen in uw account via 'Mijn Boeking' voor:</div>");
                        sb.Append("<ul>");
                        sb.Append("<li>inzage in uw boekingsgegevens/factuur</li>");
                        sb.Append("<li>een overzicht van uw Ticks spaarpunten</li>");
                        sb.Append("<li>inzage in uw betaalhistorie</li>");
                        sb.Append("</ul>");
                        sb.Append("<div>Met uw eigen ledenaccount kunt u bij elke boeking die u maakt Ticks spaarpunten winnen. Voor elke Euro die u spendeert, krijgt u 1 Tick.</div>");
                        sb.Append("<br>");
                        sb.Append("<div>Voor elke 500 Ticks kunt u kiezen om deze Ticks in te wisselen voor € 2 korting of een cadeau.</div>");
                        sb.Append("<div>Met vriendelijke groet,</div>");
                        sb.Append("<div>Team Ticketvoordeel.nl</div>");
                        sb.Append("<div>Tel.: 0031-70-44 527 48</div>");
                        sb.Append("<div>info@ticketvoordeel.nl</div>");

                        data = new TextPart("html")
                        {
                            Text = sb.ToString()
                        };

                        new EmailHelper().SendMail(profile.Email, "Uw inloggegevens voor ticketvoordeel", data);
                    }
                    catch (Exception ex)
                    {

                    }
                }                                
            }
            catch (Exception ex)
            {
                LogToFile.writeLog("RegisterUser : " + ex.Message);
            }
        }

        public string RandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public string GetCurrentTimeAsString()
        {
            DateTime currentTime = DateTime.Now;

            return (
                currentTime.Day + "-" +
                currentTime.Month + "-" +
                currentTime.Year + " " +
                currentTime.Hour + " " +
                currentTime.Minute + " " +
                currentTime.Second + " "
                );
        }

        #endregion
    }
}
