using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ticketvoordeel.Models.BookRequest;
using Ticketvoordeel.Utils;

namespace Ticketvoordeel.Helpers
{
    public class SmsHelper
    {
        public async Task<bool> SendConfirmationAsync(Message message)
        {
            using (var turSysClientList = new HttpClient())
            {
                string token = Constants.SmsApiKey;
                string responseBody = String.Empty;

                turSysClientList.BaseAddress = new Uri(Constants.SmsApiURL);
                turSysClientList.DefaultRequestHeaders.Accept.Clear();
                turSysClientList.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                turSysClientList.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage turSysResponse = await turSysClientList.PostAsync("messages", new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));
                if (turSysResponse.IsSuccessStatusCode)
                {
                    responseBody = await turSysResponse.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string GenerateSMSContent(BookResponse bookResponse, bool isRoundTrip)
        {
            if (isRoundTrip)
            {
                return "Dear " + bookResponse.Request.MainBooker.FirstName + 
                    ", your " + new DateFormatHelper().FormatDateForSMS(bookResponse.Request.DepartureBookingObject.TravelDate) + " "
                    + bookResponse.Request.travelInfo.Departure.route.OriginAirport.Code + "-" + bookResponse.Request.travelInfo.Departure.route.DestinationAirport.Code + " "
                    + new DateFormatHelper().FormatDateForSMS(bookResponse.Request.ReturnBookingObject.TravelDate) + " "
                    + bookResponse.Request.travelInfo.Arrival.route.OriginAirport.Code + "-" + bookResponse.Request.travelInfo.Arrival.route.DestinationAirport.Code + " "
                    + "ticket has been issued."
                    + "Have a good flight.PNR:"+ bookResponse.DepartureBookResponse.Pnr + "," + bookResponse.ReturnBookResponse.Pnr + ".For other services: https://ticketvoordeel.nl/login";
            }
            else
            {

                return "Dear " + bookResponse.Request.MainBooker.FirstName +
                    ", your " + new DateFormatHelper().FormatDateForSMS(bookResponse.Request.DepartureBookingObject.TravelDate) + " "
                    + bookResponse.Request.travelInfo.Departure.route.OriginAirport.Code + "-" + bookResponse.Request.travelInfo.Departure.route.DestinationAirport.Code + " "
                    + "ticket has been issued."
                    + "Have a good flight.PNR:"+ bookResponse.DepartureBookResponse.Pnr + ".For other services: https://ticketvoordeel.nl/login";
            }           
        }
    }

    public class MessageContent
    { 
        public string Name { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public string Pnr { get; set; }
        public string Phone { get; set; }
    }

    public class Message
    { 
        public string body { get; set; }
        public string encoding { get; set; } = "auto";
        public string originator { get; set; } = "Ticketvoord";
        public List<string> recipients { get; set; }
        public string route { get; set; } = "business";

        public Message()
        {
            recipients = new List<string>();
        }
    }
}
