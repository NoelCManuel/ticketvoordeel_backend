using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ticketvoordeel.Models;

namespace Ticketvoordeel.Helpers
{
    public class EmailHelper
    {
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool SendFaqRequest(string toAddress, string subject, FaqRequest faqRequest)
        {
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    //client.Connect("outlook.office365.com", 25, false);
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var messageContent = new MimeMessage();
                    messageContent.From.Add(new MailboxAddress("Ticketvoordeel Faq Request", "info@ticketvoordeel.nl"));
                    messageContent.To.Add(new MailboxAddress("Ticketvoordeel", toAddress));
                    messageContent.Subject = subject;

                    messageContent.Body = new TextPart("plain")
                    {
                        Text = @"Hi you have an faq request from : " + faqRequest.Email + @", 

                        Message: " + faqRequest.Message + @" 

                        "
                    };

                    client.Send(messageContent);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool SendMail(string toAddress, string subject, TextPart message)
        {
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    //client.Connect("smtp.werkonline.nu", 25, false);
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var messageContent = new MimeMessage();
                    messageContent.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    messageContent.To.Add(new MailboxAddress("Ticketvoordeel", toAddress));
                    messageContent.Subject = subject;
                    messageContent.Body = message;

                    client.Send(messageContent);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool SendErrorMail(string toAddress, string subject, TextPart message)
        {
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var messageContent = new MimeMessage();
                    messageContent.From.Add(new MailboxAddress("Ticketvoordeel Booking Error", "info@ticketvoordeel.nl"));
                    messageContent.To.Add(new MailboxAddress("Ticketvoordeel Booking Error", toAddress));
                    messageContent.Cc.Add(new MailboxAddress("Ticketvoordeel Booking Error", "ekrem@ticketvoordeel.nl"));
                    messageContent.Cc.Add(new MailboxAddress("Ticketvoordeel Booking Error", "cagatay@adotravel.nl"));
                    messageContent.Cc.Add(new MailboxAddress("Ticketvoordeel Booking Error", "developer@adotravel.nl"));
                    messageContent.Subject = subject;
                    messageContent.Body = message;

                    client.Send(messageContent);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void SendEmail(string subject, string content)
        {
            try
            {
                LogToFile.writeLog("Register : Mail send started");
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(subject, "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));                   
                    message.Subject = subject;
                    message.Body = new TextPart("html")
                    {
                        Text = content
                    };

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // ErrorLogger.LogError(ex, "SendEmail");
                LogToFile.writeLog(ex.ToString() + "SendEmail");
            }
        }

        public string ProcessMailInBulk(string subject, IWebHostEnvironment webHostEnvironment, string totalCredit, string creditInAmount, string name, string mail)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mail));
                    message.Subject = subject;

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();
                    var image1 = builder.LinkedResources.Add(rootFolder + "\\template\\img\\booknow.JPG");
                    image1.ContentId = MimeUtils.GenerateMessageId();


                    builder.HtmlBody = string.Format(@"<img src=""cid:{0}""><html><head></head><body><p><b>UPDATE: Jouw gespaarde Ticks</b></p><p>Gefeliciteerd! Je hebt genoeg aantal Ticks vergaard die je kan inwisselen voor een mooie korting bij je eerstvolgende boeking.</p><p>Om je &lsquo;Ticks&rsquo; saldo te bekijken, kan je inloggen in <a style='text-decoration: underline;color: black;' href=""https://www.ticketvoordeel.nl/login"">je account.</a></p><p>Je actuele &lsquo;Tick&rsquo; credits:{1} Ticks</p><p>Totale inwisselwaarde: &euro;{2} - korting</p><br><p><b>HOE WISSEL JE TICKS SPAARPUNTEN IN?</b></p><p>Elke keer dat je een boeking plaatst door eerst in te loggen met je gebruikersaccount, spaar je automatisch Tick credits.</p><p>De aantal gespaarde Tick credits zie je ook terug in je gebruikersaccount.</p><p>Je ontvangt per Euro die je besteedt, &eacute;&eacute;n Tick.</p><p>Per 500 Ticks krijg je &euro; 2,- eenmaal korting op je boeking.</p><p>Wanneer je 500 Ticks hebt gespaard, mag je ze gaan inzetten.</p><p>Ticks kan je in de laatste stap van het boekingsformulier invoeren. Dit kan alleen als je ingelogd bent.</p><br><p>Boek nu voordelig je vliegticket en wissel je Tick credits in voor een korting:</p><a style='text-decoration: underline;color: black;' href='https://www.ticketvoordeel.nl/vliegtickets'><img src=""cid:{3}""></a><div style='display:flex;padding-top: 16px;'><a style='text-decoration: underline;color:black' href=""https://ticketvoordeel.nl/veelgestelde-vragen"">Veel gestelde vragen</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a style='text-decoration: underline;color:black;' href=""https://ticketvoordeel.nl/contact"">Contact</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a style='text-decoration: underline;color:black;' href=""https://ticketvoordeel.nl/page/voorwaarden"">Voorwaarden</a></div></body></html>", image.ContentId, totalCredit, creditInAmount, image1.ContentId);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string CreditNotficationMail(string rewardPoints, string creditEarned, string name, string mailAddress, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mailAddress));
                    message.Subject = "Aantal bijgewerkte Ticks " + "(" + rewardPoints + ")";

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();



                    builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><head></head><body><img src='cid:{0}'> <p>Gefeliciteerd! Je hebt Ticks vergaard die je kan inwisselen voor een mooie korting bij een eventuele&nbsp; volgende boeking.</p> <p>Om je &lsquo;Ticks&rsquo; saldo te bekijken, kan je inloggen in&nbsp;<a href='https://www.ticketvoordeel.nl/login' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.ticketvoordeel.nl/login&amp;source=gmail&amp;ust=1664518572186000&amp;usg=AOvVaw3T8-3bG1k0oxntn2H54dPU'>je account.</a></p> <p>Bijgeschreven Ticks: " + creditEarned +" Ticks&nbsp;</p> <p>Je nieuwe saldo&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : "+ rewardPoints +" Ticks&nbsp;</p> <p>&nbsp;</p> <p><strong>HOE WISSEL JE TICKS SPAARPUNTEN IN?</strong></p> <p>Elke keer dat je een boeking plaatst door eerst in te loggen met je gebruikersaccount, spaar je automatisch Tick credits.</p> <p>De aantal gespaarde Tick credits zie je ook terug in je gebruikersaccount.</p> <p>Je ontvangt per Euro die je besteedt, &eacute;&eacute;n Tick.</p> <p>Per 500 Ticks krijg je &euro; 2,- eenmaal korting op je boeking.</p> <p>Wanneer je 500 Ticks hebt gespaard, mag je ze gaan inzetten.</p> <p>Ticks kan je in de laatste stap van het boekingsformulier invoeren. Dit kan alleen als je ingelogd bent.</p> <p>&nbsp;</p> <p>Bedankt voor uw vertrouwen in team Ticketvoordeel.nl.</p> <p>Met vriendelijke groet,</p> <p>&nbsp;</p> <p>Team Ticketvoordeel.nl</p> <p><a href='https://ticketvoordeel.nl/veelgestelde-vragen' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://ticketvoordeel.nl/veelgestelde-vragen&amp;source=gmail&amp;ust=1664518572187000&amp;usg=AOvVaw2tT0MgZb78JvPOMZkBqz52'>Veel gestelde vragen</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href='https://ticketvoordeel.nl/contact' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://ticketvoordeel.nl/contact&amp;source=gmail&amp;ust=1664518572187000&amp;usg=AOvVaw2AJ-BGDSu8WzMvakB7_LAN'>Contact</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href='https://ticketvoordeel.nl/page/voorwaarden' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://ticketvoordeel.nl/page/voorwaarden&amp;source=gmail&amp;ust=1664518572187000&amp;usg=AOvVaw3emeaPOQ64U4O6xBg_mmsb'>Voorwaarden</a></p> </body> </html>", image.ContentId);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SubscriptionMailNotify(string name, string mailAddress, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mailAddress));
                    message.Subject = "Bedankt voor je aanmelding";

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();



                    builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><body><a href='https://ticketvoordeel.nl' target='_blank'> <img src='cid:{0}'></a><p>Beste {1},</p><p>Wat leuk dat je je hebt aangemeld voor onze nieuwsbrief met de beste vliegticket aanbiedingen.</p><p>Je ontvangt nu automatisch de beste aanbiedingen per e-mail.</p><p>Onderaan elke nieuwsbrief die je ontvangt, tref je ook een &lsquo;uitschrijf&rsquo; link. Door hierop te klikken kan je je elk moment gaan afmelden.</p><p>Wist je dat je ook bij elke nieuwe boeking Ticks credits bij ons kunt sparen?</p><br><p>De aantal gespaarde Tick credits zie je ook terug in je gebruikersaccount.</p><p>Je ontvangt per Euro die je besteedt, &eacute;&eacute;n Tick.</p><p>Per 500 Ticks krijg je &euro; 2,- eenmaal korting op je boeking.</p><p>Wanneer je 500 Ticks hebt gespaard, mag je ze gaan inzetten.</p><p>Hopelijk tot binnenkort op een van onze mooie bestemmingen.</p><br><p>Met vriendelijke groet,</p><p>Suzie namens <a href='https://ticketvoordeel.nl' target='_blank'>Ticketvoordeel.nl</a></p></body></html>", image.ContentId, name);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SubscriptionMailNotifyForTV(string name, string mailAddress, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.Subject = "Newsletter Subscribed!";

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();



                    builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><body><img src='cid:{0}'><p>Hi, User just registered for newsletter!</p><p>Name: {1}</p><p>Email: {2}</p></body></html>", image.ContentId, name, mailAddress);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string ProcessMailForIncompleteBooking(IWebHostEnvironment webHostEnvironment, string name, string mail,string typeUser, IncompleteBooking incompleteBooking)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;
                if(!string.IsNullOrEmpty(name))
                {
                    name = char.ToUpper(name[0]) + name.Substring(1);
                }
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mail));

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();
                    message.Subject = "Onvolledige boeking";

                    if (typeUser == "user")
                    {
                        builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><body><img width='150px' src='cid:{0}'><p>Beste {1},</p>

                        <p>Wij hopen dat u deze e-mail u in goede orde ontvangt.</p>
                        <p>We hebben gemerkt dat u uw boeking niet heeft kunnen afronden. We willen er graag voor zorgen dat u nog steeds de mogelijkheid krijgt om de boeking alsnog met succes af te ronden.</p>
                        <p>U kunt uw boeking proberen nogmaals te voltooien via onderstaande link:</p>
                        <a href='{2}'>www.TicketVoordeel.nl</a>
                        <p>Bedankt voor het kiezen voor Ticketvoordeel.</p>
                        <p>Graag vernemen we van u of we u verder kunnen helpen met uw boeking.</p>
                        <p>Met vriendelijke groet,</p>
                        <p>Suzie namens <a href='https://www.ticketvoordeel.nl/vliegtickets' target='_blank'>TicketVoordeel.nl</a></p>

                        </body>
                        </html>", image.ContentId, name, "https://www.ticketvoordeel.nl/vliegtickets");
                    }
                    else if (typeUser == "admin")
                    {
                        message.Subject = "Onvolledige boeking";

                        #region STEP IMAGE PATH
                        if (!string.IsNullOrEmpty(incompleteBooking.steponepath))
                        {
                            var image1 = builder.Attachments.Add(incompleteBooking.steponepath);
                            image1.ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment);
                            image1.ContentDisposition.FileName = Path.GetFileName(incompleteBooking.steponepath);
                            image1.ContentType.Name = Path.GetFileName(incompleteBooking.steponepath);
                        }
                        if (!string.IsNullOrEmpty(incompleteBooking.steptwopath))
                        {
                            var image2 = builder.Attachments.Add(incompleteBooking.steptwopath);
                            image2.ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment);
                            image2.ContentDisposition.FileName = Path.GetFileName(incompleteBooking.steptwopath);
                            image2.ContentType.Name = Path.GetFileName(incompleteBooking.steptwopath);
                        }                        
                        if (!string.IsNullOrEmpty(incompleteBooking.stepfourpath))
                        {
                            var image3 = builder.Attachments.Add(incompleteBooking.stepfourpath);
                            image3.ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment);
                            image3.ContentDisposition.FileName = Path.GetFileName(incompleteBooking.stepfourpath);
                            image3.ContentType.Name = Path.GetFileName(incompleteBooking.stepfourpath);
                        }
                        if (!string.IsNullOrEmpty(incompleteBooking.stepthreepath))
                        {
                            var image3 = builder.Attachments.Add(incompleteBooking.stepthreepath);
                            image3.ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment);
                            image3.ContentDisposition.FileName = Path.GetFileName(incompleteBooking.stepthreepath);
                            image3.ContentType.Name = Path.GetFileName(incompleteBooking.stepthreepath);
                        }

                        #endregion

                        #region Passenger Info

                        var result = string.Empty;
                        var otherInfo = string.Empty;

                        if (!string.IsNullOrEmpty(incompleteBooking.personalDetails))
                        {
                            var viewPersonalDetails = JsonConvert.DeserializeObject<IncompleteBookingParams>(incompleteBooking.personalDetails);
                            if (viewPersonalDetails != null)
                            {
                                result = "<table>" + Environment.NewLine;
                                result += "<thead>" + Environment.NewLine;
                                result += "<tr>" + Environment.NewLine;
                                result += "<th>Title</th>" + Environment.NewLine;
                                result += "<th>First Name</th>" + Environment.NewLine;
                                result += "<th>Last Name</th>" + Environment.NewLine;
                                result += "<th>Birth Date</th>" + Environment.NewLine;
                                result += "<th>Identity Number</th>" + Environment.NewLine;
                                result += "<th>Phone</th>" + Environment.NewLine;
                                result += "<th>Email</th>" + Environment.NewLine;
                                result += "<th>City</th>" + Environment.NewLine;
                                result += "<th>Postcode</th>" + Environment.NewLine;
                                result += "<th>House Number</th>" + Environment.NewLine;
                                result += "<th>Place</th>" + Environment.NewLine;

                                result += "</tr>" + Environment.NewLine;
                                result += "</thead>" + Environment.NewLine;
                                result += "<tbody>" + Environment.NewLine;
                                foreach (var passenger in viewPersonalDetails.finalPassengersList)
                                {
                                    result += "<tr>" + Environment.NewLine;
                                    result += string.Format("<td>{0}</td>", passenger.Title);
                                    result += string.Format("<td>{0}</td>", passenger.FirstName);
                                    result += string.Format("<td>{0}</td>", passenger.LastName);
                                    result += string.Format("<td>{0}</td>", passenger.Birthdate);
                                    result += string.Format("<td>{0}</td>", passenger.IdentityNumber ?? "N/A");
                                    result += string.Format("<td>{0}</td>", Convert.ToString(viewPersonalDetails.contactDetails.mobile) ?? "N/A");
                                    result += string.Format("<td>{0}</td>", viewPersonalDetails.contactDetails.email ?? "N/A");
                                    result += string.Format("<td>{0}</td>", viewPersonalDetails.contactDetails.streetName ?? "N/A");
                                    result += string.Format("<td>{0}</td>", viewPersonalDetails.contactDetails.postelCode ?? "N/A");
                                    result += string.Format("<td>{0}</td>", viewPersonalDetails.contactDetails.houseNumber ?? "N/A");
                                    result += string.Format("<td>{0}</td>", viewPersonalDetails.contactDetails.Place ?? "N/A");

                                    result += "</tr>" + Environment.NewLine;
                                }

                                result += "</tbody>" + Environment.NewLine;
                                result += "</table>" + Environment.NewLine;
                            }
                        }
                        #endregion

                        #region Body
                        builder.HtmlBody = string.Format(@"<!DOCTYPE html>
                            <html>
                            <head>
                            <style>
                            table, th, td {{
                              border: 1px solid black;
                            }}
                            th, td {{
                              padding: 15px;
                            }}
                            </style>
                            </head>
                            <body>
                            <a href='https://www.ticketvoordeel.nl/vliegtickets' target='_blank'><img width='150px' src='cid:{0}'></a>
                            <p>Beste {1},</p>
                            <p>Incomplete Booking Details</p>
                            <div>{2}</div><br>
                            <div>{3}</div>
                            <br>
                            <p>Met vriendelijke groet,</p><p>Suzie namens <a href='https://www.ticketvoordeel.nl/vliegtickets' target='_blank'>www.ticketvoordeel.nl</a></p>
                            </body>
                            </html>",
                            image.ContentId,
                            name,
                            result,
                            otherInfo
                            );
                        #endregion
                    }

                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SubscriptionMailForSunExpress(string name, string mailAddress, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mailAddress));
                    //message.To.Add(new MailboxAddress("Ticketvoordeel", "claudious.neol@polussoftware.com"));
                    message.Subject = "Online check-in SunExpress";

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();

                    var image1 = builder.LinkedResources.Add(rootFolder + "\\template\\img\\sunexpress.png");
                    image1.ContentId = MimeUtils.GenerateMessageId();



                    builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><body>
                    <p>Beste passagier,</p>
                    <p>Via de onderstaande link dient u online in te checken voor uw vlucht van SunExpress (uw heenvlucht of uw terugvlucht of allebei indien van toepassing).</p>
                    <p style='color:red;'><u style='text-decoration-color:black;'><b>Online check in kan vanaf 36 uur voor vertrek tot 3 uur voor vertrek.</b></u></p>
                    <p><b>UITZONDERING:</b> Als u rolstoelservice of assistentie heeft bijgeboekt, dan kunt u NIET online inchecken. Check-in zal dan kosteloos op de airport plaatsvinden.</p>
                    <p>Als u op de airport incheckt, zal de airlines € 5 p.p. kosten in rekening brengen.</p>
                    <p><a href='https://www.sunexpress.com/en/online-check-in/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.sunexpress.com/en/online-check-in/&amp;source=gmail&amp;ust=1681551735896000&amp;usg=AOvVaw1Ocxa8MF2Q88g8qY3eiCw2'>Online check-in - SunExpress</a></p>
                    <p>Vervolgens selecteert u <b>‘’Sunexpress‘’</b> en voert u bij <b>‘’PNR’’</b> uw <b>AIRLINE</b> reserveringsnummer in.</p>
                    <p>Deze airline res. nr. staat op uw factuur en ook op uw ticket vermeld.</p>
                    <p>De voornaam en achternaam van de eerste passagier dient u met KLEINE letters in te vullen! Mocht dit niet werken, dan kunt u het met hoofdletters proberen.</p>
                    <p>Volg de instructies op uw scherm en voer alle gegevens in zoals op uw factuur/bevestiging staat vermeld. Gebruik geen speciale tekens, enkel letters van de Europese alfabet.</p>
                    <img src='cid:{1}'>
                    <p>Indien u op de airport incheckt brengt de Airlines 5 euro kosten per persoon in rekening.</p>
                    <p>Wij wensen u hierbij een fijne vlucht toe!</p>
                    <p>Met vriendelijke groet / With kind regards,</p>
                    <p>Team Ticketvoordeel.nl</p>

                    <img src='cid:{0}'>

                    <p style='margin:0'>Laan van Oversteen 2-18</p>
                    <p style='margin:0'>2289 CX Rijswijk</p>
                    <p style='margin:0'>T.: +31(0)70- 44 527 48</p>
                    <p style='margin:0'>E.: info@ticketvoordeel.nl</p>
                    </body></html>", image.ContentId, image1.ContentId);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SubscriptionMailForSunExpressAdotravel(string name, string mailAddress, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                var rootFolder = webHostEnvironment.WebRootPath;

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("arg-plplcl11.argewebhosting.nl", 587, false);
                    client.Authenticate("info@ticketvoordeel.nl", "2020TickRijswijk@");

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Ticketvoordeel", "info@ticketvoordeel.nl"));
                    message.To.Add(new MailboxAddress(name, mailAddress));
                    //message.To.Add(new MailboxAddress("Ticketvoordeel", "claudious.neol@polussoftware.com"));
                    message.Subject = "Online check-in SunExpress";

                    var builder = new BodyBuilder();
                    var image = builder.LinkedResources.Add(rootFolder + "\\template\\img\\logoo.JPG");
                    image.ContentId = MimeUtils.GenerateMessageId();

                    var image1 = builder.LinkedResources.Add(rootFolder + "\\template\\img\\sunexpress.png");
                    image1.ContentId = MimeUtils.GenerateMessageId();



                    builder.HtmlBody = string.Format(@"<!DOCTYPE html><html><body>
                    <p>Beste passagier,</p>
                    <p>Via de onderstaande link dient u online in te checken voor uw vlucht van SunExpress (uw heenvlucht of uw terugvlucht of allebei indien van toepassing).</p>
                    <p style='color:red;'><u style='text-decoration-color:black;'><b> Online check in kan vanaf 36 uur voor vertrek tot 3 uur voor vertrek.</b></u></p>
                    <p>Als u rolstoelassistentie bij Sunexpress heeft aangevraagd, kunt u <b>NIET online inchecken</b>. Dan dient check in op de airport plaats te vinden bij de check-in balie.</p>
                    <p>LET OP: Als u op de airport incheckt, zal de airlines kosten in rekening brengen.</p>

                    <p><a href='https://www.sunexpress.com/en/online-check-in/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.sunexpress.com/en/online-check-in/&amp;source=gmail&amp;ust=1681551735896000&amp;usg=AOvVaw1Ocxa8MF2Q88g8qY3eiCw2'>Online check-in - SunExpress</a></p>
                    <p>Vervolgens selecteert u <b>‘’tour operator‘’</b> en voert u bij <b>‘’tour operator reference number’’</b> uw adotravel reserveringsnummer (combinatie van letters en cijfers).</p>
                    <p>De voornaam en achternaam van de eerste passagier dient u met KLEINE letters in te vullen! Mocht dit niet lukken, probeert u het ook door hoofdletters te gebruiken.</p>
                    <p>Volg de instructies op uw scherm en voer alle gegevens in zoals die op uw factuur/bevestiging ook staan vermeld. Gebruik geen speciale tekens, enkel letters van de Europese alfabet.</p>

                    <img src='cid:{1}'>
                    <p>Indien u op de airport incheckt brengt de airlines € 5,- per persoon kosten in rekening.</p>
                    <p>Wij wensen u hierbij een fijne vakantie toe!</p>
                    <p>Met vriendelijke groet,</p>
                    <p>Team Adotravel</p>

                    <img src='cid:{0}'>

                    <p style='margin:0'>Laan van Oversteen 2-18</p>
                    <p style='margin:0'>2289 CX Rijswijk</p>
                    <p style='margin:0'>T.: +31(0)70- 44 527 48</p>
                    <p style='margin:0'>E.: info@ticketvoordeel.nl</p>
                    </body></html>", image.ContentId, image1.ContentId);
                    message.Body = builder.ToMessageBody();

                    client.Send(message);
                    client.Disconnect(true);
                }
                return "true";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
