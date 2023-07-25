using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ticketvoordeel.Entities;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.AirpoolSearchResponse;
using Ticketvoordeel.Models.Booking;
using Ticketvoordeel.Models.BookRecap;
using Ticketvoordeel.Models.BookRequest;
using Ticketvoordeel.Models.CreateBasket;
using Ticketvoordeel.Models.ParkingResponse;
using Ticketvoordeel.Models.PriceRequest;
using Ticketvoordeel.Models.PriceRequests;
using Ticketvoordeel.Models.PricingResponse;
using Ticketvoordeel.Models.TravelInfo;
using Ticketvoordeel.Models.TwoWayRequest;
using Ticketvoordeel.Payment;
using Ticketvoordeel.Utils;

namespace Ticketvoordeel.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FlightPoolController : Controller
    {
        private IWebHostEnvironment environment;
        private IRepositoryWrapper _repository;
        public FlightPoolController(IWebHostEnvironment _environment, IRepositoryWrapper repository)
        {
            environment = _environment;
            _repository = repository;
        }

        [Route("/airpool/airports")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Airports([FromBody]AirportSearchViewModel airportSearchViewModel)
        {
            try
            {
                LogToFile.writeLog("Airport request started");
                return Json(new Response { Data = JObject.Parse(new ApiRequestHelper().GetData("api/airpool/airports", airportSearchViewModel).Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = airportSearchViewModel, Message = "error", Status = false });
            }
        }

        [Route("/airpool/search")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Search([FromBody]PoolRequestBaseViewModel poolRequestBaseViewModel)
        {
            try
            {
                var tt = JsonConvert.SerializeObject(poolRequestBaseViewModel);
                string response = new StringFormatterHelper().ConvertIdAndRouteIdToString(new ApiRequestHelper().GetData("/api/airpool/search", poolRequestBaseViewModel).Result);
                if (!string.IsNullOrEmpty(response))
                {
                    return Json(new Response { Data = JObject.Parse(response), Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = null, Message = "error", Status = true });
                }
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = poolRequestBaseViewModel, Message = "error", Status = false });
            }
        }

        [Route("/airpool/twowaysearch")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult TwoWaySearch([FromBody]TwoWayRequest twoWayRequest)
        {
            PriceResponses priceResponses = new PriceResponses();
            priceResponses.IsCombined = false;
            var isTKFlight = false;
            try
            {
                #region TWO WAY SOLUTION SUGGESTED BY TURKISK DEVELOPERS
                if (twoWayRequest != null)
                {
                    if(twoWayRequest.SelectedAirlines != null)
                    {
                        var getFlightNumber = string.Empty;
                        List<SelectedFlights> selectedFlightsList = new List<SelectedFlights>();

                        if (twoWayRequest.SelectedAirlines.DepartureAirlines != null)
                        {                           
                            foreach (var eachFlight in twoWayRequest.SelectedAirlines.DepartureAirlines)
                            {
                                StringFormatterHelper stringFormatterHelper = new StringFormatterHelper();
                                SelectedFlights selectedFlights = new SelectedFlights();
                                getFlightNumber = stringFormatterHelper.RemoveFlightNumber(eachFlight.AirlineCode);
                                if (!string.IsNullOrEmpty(getFlightNumber) && getFlightNumber != "NOTK")
                                {
                                    isTKFlight = true;
                                    selectedFlights.FlightNumber = getFlightNumber;
                                    selectedFlights.FareClass = eachFlight.AirlineClass;
                                    selectedFlightsList.Add(selectedFlights);
                                    break;
                                }
                            }
                        }

                        twoWayRequest.PoolRequest.Preference.SelectedFlights = selectedFlightsList;
                    }
                }
                #endregion

                AirpoolSearchResponseViewModel airpoolSearchResponseViewModel = new AirpoolSearchResponseViewModel();
                var response = new ApiRequestHelper().GetData("/api/airpool/search", twoWayRequest).Result;
                var tt = JsonConvert.SerializeObject(twoWayRequest);
                airpoolSearchResponseViewModel = JsonConvert.DeserializeObject<AirpoolSearchResponseViewModel>(response);
                List<List<Models.AirpoolSearchResponse.Route>> departureRoutes = new List<List<Models.AirpoolSearchResponse.Route>>();
                List<List<Models.AirpoolSearchResponse.Route>> returnRoutes = new List<List<Models.AirpoolSearchResponse.Route>>();
                List<FareGroup> DepartureFareGroups = new List<FareGroup>();
                List<FareGroup> ReturnFareGroups = new List<FareGroup>();    

                foreach (var items in airpoolSearchResponseViewModel.Reader.FareGroups)
                {
                    if (items.Routes.Count == 2)
                    {
                        if (items.Routes[0][0].Segments.Count == twoWayRequest.SelectedAirlines.DepartureAirlines.Count)
                        {
                            bool isValid = true;
                            for (int i = 0; i < items.Routes[0][0].Segments.Count; i++)
                            {
                                if (items.Routes[0][0].Segments[i].Number != twoWayRequest.SelectedAirlines.DepartureAirlines[i].AirlineCode)
                                {
                                    isValid = false;
                                }
                                if (items.Routes[0][0].FareClass != twoWayRequest.SelectedAirlines.DepartureAirlines[i].AirlineClass)
                                {
                                    isValid = false;
                                }
                            }
                            if (isValid)
                            {
                                DepartureFareGroups.Add(items);
                            }
                        }

                        if (isTKFlight)
                        {
                            var secondSegment = items.Routes[1];
                            for (int i = 0; i < secondSegment.Count; i++)
                            {
                                if (secondSegment[i].Segments.Count == twoWayRequest.SelectedAirlines.ReturnAirlines.Count)
                                {
                                    bool isValid1 = true;

                                    for (int j = 0; j < secondSegment[i].Segments.Count; j++)
                                    {
                                        if (secondSegment[i].Segments[j].Number != twoWayRequest.SelectedAirlines.ReturnAirlines[j].AirlineCode)
                                        {
                                            isValid1 = false;
                                        }
                                        if (secondSegment[i].FareClass != twoWayRequest.SelectedAirlines.ReturnAirlines[j].AirlineClass)
                                        {
                                            isValid1 = false;
                                        }
                                    }
                                    if (isValid1)
                                    {
                                        DepartureFareGroups.Add(items);
                                    }
                                }
                            }                          
                        }
                        
                    }
                }

                foreach (var item in DepartureFareGroups)
                {
                    if (item.Routes[1][0].Segments.Count == twoWayRequest.SelectedAirlines.ReturnAirlines.Count)
                    {
                        bool isValid = false;
                        for (int i = 0; i < item.Routes[1][0].Segments.Count; i++)
                        {
                            if (item.Routes[1][0].Segments[i].Number == twoWayRequest.SelectedAirlines.ReturnAirlines[i].AirlineCode && item.Routes[1][0].FareClass == twoWayRequest.SelectedAirlines.ReturnAirlines[i].AirlineClass)
                            {
                                isValid = true;
                            }
                            //if (item.Routes[1][0].FareClass != twoWayRequest.SelectedAirlines.ReturnAirlines[i].AirlineClass)
                            //{
                            //    isValid = false;
                            //}
                        }
                        if (isValid)
                        {
                            ReturnFareGroups.Add(item);
                        }
                    }
                }

                foreach (var item in DepartureFareGroups)
                {
                    if (isTKFlight)
                    {
                        var secondSegment = item.Routes[1];
                        var secondSegmentClone = new List<Models.AirpoolSearchResponse.Route>();

                        if (secondSegment != null)
                        {
                            foreach (var newItems in secondSegment)
                            {
                                secondSegmentClone.Add(newItems);
                            }
                        }

                        for (int i = 0; i < secondSegment.Count; i++)
                        {
                            if (secondSegment[i].Segments.Count == twoWayRequest.SelectedAirlines.ReturnAirlines.Count)
                            {
                                bool isValid1 = false;

                                for (int j = 0; j < secondSegment[i].Segments.Count; j++)
                                {
                                    if (secondSegment[i].Segments[j].Number == twoWayRequest.SelectedAirlines.ReturnAirlines[j].AirlineCode && secondSegment[i].FareClass == twoWayRequest.SelectedAirlines.ReturnAirlines[j].AirlineClass)
                                    {
                                        isValid1 = true;
                                    }
                                    else
                                    {
                                        if (secondSegmentClone.Count != 0)
                                        {
                                            secondSegmentClone.RemoveAt(0);
                                        }
                                    }
                                    //if (secondSegment[i].FareClass != twoWayRequest.SelectedAirlines.ReturnAirlines[j].AirlineClass)
                                    //{
                                    //    isValid1 = false;
                                    //}
                                }
                                if (isValid1)
                                {
                                    item.Routes[1] = secondSegmentClone;
                                    ReturnFareGroups.Add(item);
                                }
                            }
                        }
                    }
                }

                if (ReturnFareGroups.Count == 1)
                {
                    decimal Totalcombinedamount = ReturnFareGroups[0].TotalPrice.Total;
                    if (Totalcombinedamount < twoWayRequest.SelectedAirlines.TotalPrice)
                    {
                        AirpoolPricingRequestViewModel airpoolPricingRequestViewModel = new AirpoolPricingRequestViewModel();
                        Models.PriceRequest.SelectedRoute selectedDepartureRoute = new Models.PriceRequest.SelectedRoute();
                        Models.PriceRequest.SelectedRoute selectedReturnRoute = new Models.PriceRequest.SelectedRoute();
                        airpoolPricingRequestViewModel.PoolRequest = new Models.PriceRequest.PoolRequest();
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes = new List<Models.PriceRequest.SelectedRoute>();
                        airpoolPricingRequestViewModel.CurrencyCode = "EUR";
                        airpoolPricingRequestViewModel.PoolRequest.SearchId = airpoolSearchResponseViewModel.Reader.SearchId.ToString();
                        airpoolPricingRequestViewModel.PoolRequest.TripId = ReturnFareGroups[0].Id;

                        selectedDepartureRoute.RouteId = ReturnFareGroups[0].Routes[0][0].Id;
                        selectedDepartureRoute.RouteFareOptionId = ReturnFareGroups[0].Routes[0][0].RouteFareOptionId;
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes.Add(selectedDepartureRoute);
                        selectedReturnRoute.RouteId = ReturnFareGroups[0].Routes[1][0].Id;
                        selectedReturnRoute.RouteFareOptionId = ReturnFareGroups[0].Routes[1][0].RouteFareOptionId;
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes.Add(selectedReturnRoute);

                        var result = new ApiRequestHelper().GetData("/api/airpool/pricing", airpoolPricingRequestViewModel).Result;
                        var capResult = JsonConvert.DeserializeObject<PricingResponse>(result);
                        var joinRoutId = string.Empty;
                        var joinRoutFareId = string.Empty;
                        if (capResult != null)
                        {
                            var RouteDepId = capResult.Selection.BookingGroups[0].Routes[0].Id;
                            var RouteRetId = capResult.Selection.BookingGroups[0].Routes[1].Id;
                            joinRoutId = RouteDepId + ":" + RouteRetId;

                            var RouteDepFareId = capResult.Selection.BookingGroups[0].Routes[0].RouteFareOptionId;
                            var RouteRetFareId = capResult.Selection.BookingGroups[0].Routes[1].RouteFareOptionId;
                            joinRoutFareId = RouteDepFareId + ":" + RouteRetFareId;
                        }
                        return Json(new Response { Data = JObject.Parse(result), Message = "success", Status = true, Others1 = joinRoutId, Others2 = joinRoutFareId });
                        //PricingResponse pricingResponse = new PricingResponse();
                        //pricingResponse = JsonConvert.DeserializeObject<PricingResponse>(result);
                        //priceResponses.IsCombined = true;
                        //priceResponses.PricingResponse = pricingResponse;
                    }
                }
                else if(ReturnFareGroups.Count > 1)
                {
                    var maxPrice = ReturnFareGroups.Max(x => x.TotalPrice).Total;
                    var simple = ReturnFareGroups.Where(c=>c.TotalPrice.Total == maxPrice).ToList();
                    if(simple.Count > 0)
                    {
                        ReturnFareGroups = simple;
                    }
                    decimal Totalcombinedamount = ReturnFareGroups[0].TotalPrice.Total;
                    if (Totalcombinedamount < twoWayRequest.SelectedAirlines.TotalPrice)
                    {
                        AirpoolPricingRequestViewModel airpoolPricingRequestViewModel = new AirpoolPricingRequestViewModel();
                        Models.PriceRequest.SelectedRoute selectedDepartureRoute = new Models.PriceRequest.SelectedRoute();
                        Models.PriceRequest.SelectedRoute selectedReturnRoute = new Models.PriceRequest.SelectedRoute();
                        airpoolPricingRequestViewModel.PoolRequest = new Models.PriceRequest.PoolRequest();
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes = new List<Models.PriceRequest.SelectedRoute>();
                        airpoolPricingRequestViewModel.CurrencyCode = "EUR";
                        airpoolPricingRequestViewModel.PoolRequest.SearchId = airpoolSearchResponseViewModel.Reader.SearchId.ToString();
                        airpoolPricingRequestViewModel.PoolRequest.TripId = ReturnFareGroups[0].Id;

                        selectedDepartureRoute.RouteId = ReturnFareGroups[0].Routes[0][0].Id;
                        selectedDepartureRoute.RouteFareOptionId = ReturnFareGroups[0].Routes[0][0].RouteFareOptionId;
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes.Add(selectedDepartureRoute);
                        selectedReturnRoute.RouteId = ReturnFareGroups[0].Routes[1][0].Id;
                        selectedReturnRoute.RouteFareOptionId = ReturnFareGroups[0].Routes[1][0].RouteFareOptionId;
                        airpoolPricingRequestViewModel.PoolRequest.SelectedRoutes.Add(selectedReturnRoute);

                        var result = new ApiRequestHelper().GetData("/api/airpool/pricing", airpoolPricingRequestViewModel).Result;
                        var capResult = JsonConvert.DeserializeObject<PricingResponse>(result);
                        var joinRoutId = string.Empty;
                        var joinRoutFareId = string.Empty;
                        if (capResult != null)
                        {
                            var RouteDepId = capResult.Selection.BookingGroups[0].Routes[0].Id;
                            var RouteRetId = capResult.Selection.BookingGroups[0].Routes[1].Id;
                            joinRoutId = RouteDepId + ":" + RouteRetId;

                            var RouteDepFareId = capResult.Selection.BookingGroups[0].Routes[0].RouteFareOptionId;
                            var RouteRetFareId = capResult.Selection.BookingGroups[0].Routes[1].RouteFareOptionId;
                            joinRoutFareId = RouteDepFareId + ":" + RouteRetFareId;
                        }
                        return Json(new Response { Data = JObject.Parse(result), Message = "success", Status = true, Others1 = joinRoutId, Others2 = joinRoutFareId });
                        //PricingResponse pricingResponse = new PricingResponse();
                        //pricingResponse = JsonConvert.DeserializeObject<PricingResponse>(result);
                        //priceResponses.IsCombined = true;
                        //priceResponses.PricingResponse = pricingResponse;
                    }
                }


                JObject json = JObject.Parse(JsonConvert.SerializeObject(priceResponses));
                LogToFile.CheckFailureReport(JsonConvert.SerializeObject(twoWayRequest));
                return Json(new Response { Data = json, Message = "success", Status = false });
            }
            catch (Exception ex)
            {
                LogToFile.CheckFailureReport("Exception" + JsonConvert.SerializeObject(twoWayRequest));
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [Route("/airpool/pricing")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Pricing([FromBody]AirpoolPricingRequestViewModel airpoolPricingRequestViewModel)
        {
            try
            {
                return Json(new Response { Data = JObject.Parse(new ApiRequestHelper().GetData("/api/airpool/pricing", airpoolPricingRequestViewModel).Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = airpoolPricingRequestViewModel, Message = "error", Status = false });
            }
        }

        [Route("/airpool/basket/create")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult CreateBasket([FromBody]CreateBasketViewModel createBasketViewModel)
        {
            try
            {
                createBasketViewModel.Passengers = ConvertPassengerCharacters1(createBasketViewModel.Passengers);
                int PassengerCount = createBasketViewModel.Passengers.Count();
                for (int i = 0; i < PassengerCount; i++)
                {
                    createBasketViewModel.Passengers[i].IdentityNumber += (i + 1);
                }
                return Json(new Response { Data = JObject.Parse(new ApiRequestHelper().GetData("/api/airpool/basket/create", createBasketViewModel).Result), Message = "success", Status = true });
            }
            catch (Exception ex)
            {
                LogToFile.writeLog(ex.Message);
                return Json(new Response { Data = createBasketViewModel, Message = "error", Status = false });
            }
        }

        [Route("/book")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Book([FromBody] FinalBookRequest finalBookRequest)
        {
            try
            {
                LogToFile.AdminMailForCredit1(JsonConvert.SerializeObject(finalBookRequest));
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(finalBookRequest), "6");
                BookRequest bookRequest = new BookRequest();
                ErrorMailRequest errorMailRequest = new ErrorMailRequest();               
                var transactionDetail = _repository.PaymentTransactionRepository.GetPaymentTransaction(c => c.TransactionId == finalBookRequest.TransactionId).FirstOrDefault();
                bookRequest = JsonConvert.DeserializeObject<BookRequest>(transactionDetail.BookRequest);
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(transactionDetail), "7");

                bookRequest.sisowPayment = finalBookRequest.sisowPayment;
                BookResponse bookResponse = new BookResponse();
                bookResponse.Request = bookRequest;
                bookResponse.UserEmail = bookRequest.UserEmail;

                //update characters.. has to be updated later. just temporary
                bookResponse.Request.MainBooker = ConvertMainBookerCharacters(bookResponse.Request.MainBooker);
                bookResponse.Request.Passengers = ConvertPassengerCharacters(bookResponse.Request.Passengers);
                bookResponse.Request.DepartureBookingObject.contactInformation =
                    ConvertContactInformationCharacters(bookResponse.Request.DepartureBookingObject.contactInformation);
                bookResponse.Request.DepartureBookingObject.contactInformation =
                    ConvertContactInformationCharacters(bookResponse.Request.DepartureBookingObject.contactInformation);

                var booking = _repository.BookingRepository.GetBooking(c => c.TransactionId == finalBookRequest.TransactionId).FirstOrDefault();
                if (booking != null && booking.Id > 0)
                {
                    bookResponse = JsonConvert.DeserializeObject<BookResponse>(booking.BookingDetails);
                    bookResponse.TransactionId = finalBookRequest.TransactionId;
                    return Json(new Response { Data = bookResponse, Message = "exist", Status = true });
                }

                FinalBookResponse departureBookResponse = new FinalBookResponse();
                FinalBookResponse returnBookResponse = new FinalBookResponse();                
                errorMailRequest.BookRequest = bookRequest;               
                
                //Parking comes within the external items list. But parking object also should be there to book it
                //if (bookRequest.IsParkingSelected)
                //{
                //    AddExtrenalItem(bookRequest.DepartureBookingObject.BasketKey, "Parking", bookRequest.ParkingAmount, "Parking");
                //}

                //Applying external items
                if (bookRequest.ExternalItems != null)
                {
                    foreach (var items in bookRequest.ExternalItems)
                    {
                        AddExtrenalItem(bookRequest.DepartureBookingObject.BasketKey, items.Name, items.Amount, items.Name);
                    }
                }

                //Applying discounts
                if (bookRequest.Discounts != null)
                {
                    foreach (var items in bookRequest.Discounts)
                    {
                        if (items.Name.ToLower() == "Ticks".ToLower())
                        {
                            bookRequest.AppliedCredit = Convert.ToInt32(items.Amount * 250);
                        }
                        if (items.Name.ToLower() == "sov voucherkorting".ToLower())
                        {
                            bookResponse.SuvendusApplied = "YES";
                        }
                        AddExtrenalItem(bookRequest.DepartureBookingObject.BasketKey, items.Name, items.Amount * -1, items.Name);
                    }
                }

                bookRequest.DepartureBookingObject.Payment = new Models.BookRequest.Payment();

                if (bookRequest.IsReturnAvailable)
                {
                    bookRequest.ReturnBookingObject.Payment = new Models.BookRequest.Payment();
                    bookRequest.DepartureBookingObject.SalesAmount = (bookRequest.TotalAmount - Convert.ToDecimal(bookRequest.ReturnBookingObject.SalesAmount)).ToString();
                    returnBookResponse = JsonConvert.DeserializeObject<FinalBookResponse>(Prebook(bookRequest.ReturnBookingObject.BasketKey, PopulateAddress(bookRequest.ReturnBookingObject, bookRequest.MainBooker)));
                    //returnBookResponse.HasProblem = false;
                    //returnBookResponse.Pnr = "TEST_RETURN";
                    //returnBookResponse.Message = "success";
                }
                else
                {
                    bookRequest.DepartureBookingObject.SalesAmount = bookRequest.TotalAmount.ToString();
                }
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(bookRequest), "8");

                departureBookResponse = JsonConvert.DeserializeObject<FinalBookResponse>(Prebook(bookRequest.DepartureBookingObject.BasketKey, PopulateAddress(bookRequest.DepartureBookingObject, bookRequest.MainBooker)));
                //departureBookResponse.HasProblem = false;
                //departureBookResponse.Pnr = "TEST_DEPARTURE";
                //departureBookResponse.Message = "success";

                bookResponse.ReturnBookResponse = returnBookResponse;
                bookResponse.DepartureBookResponse = departureBookResponse;
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(bookResponse), "11");

                if (bookRequest.IsReturnAvailable)
                {
                    if ((departureBookResponse == null || departureBookResponse.HasProblem) &&
                        (returnBookResponse == null || returnBookResponse.HasProblem))
                    {
                        errorMailRequest.TravelDate = bookResponse.Request.DepartureBookingObject.TravelDate;
                        errorMailRequest.IsErrorInBoth = true;
                        SendErrorMail(errorMailRequest);
                    }
                    else
                    {
                        if (departureBookResponse == null || departureBookResponse.HasProblem)
                        {
                            errorMailRequest.TravelDate = bookResponse.Request.DepartureBookingObject.TravelDate;
                            errorMailRequest.IsErrorinDeparture = true;
                            SendErrorMail(errorMailRequest);
                        }
                        if (returnBookResponse == null || returnBookResponse.HasProblem)
                        {
                            errorMailRequest.TravelDate = bookResponse.Request.DepartureBookingObject.TravelDate;
                            errorMailRequest.IsErrorinDeparture = false;
                            SendErrorMail(errorMailRequest);
                        }
                    }
                    
                    if (departureBookResponse.HasProblem == false && returnBookResponse.HasProblem == false)
                    {
                        if (bookRequest.IsInsuranceSelected)
                        {
                            bookResponse.DepartureInsurancePolicyNumber = BookInsurance(bookRequest.DepartureBookingObject, bookRequest, departureBookResponse);
                            bookResponse.ReturnInsurancePolicyNumber = BookInsurance(bookRequest.ReturnBookingObject, bookRequest, returnBookResponse);
                        }
                        if (bookRequest.IsParkingSelected)
                        {
                            bookResponse.ParkingPNRList = BookParking(bookRequest, bookResponse.DepartureBookResponse.Pnr, bookRequest.parkingSelectedCount);
                        }

                        var checkSunExpressMail = false;

                        if (bookRequest.travelInfo.Departure.route.Segments != null)
                        {
                            if (bookRequest.travelInfo.Departure.route.Segments.Count() > 0)
                            {
                                if (bookRequest.travelInfo.Departure.route.Segments[0].Carrier.Name.ToLower() == "sunexpress")
                                {
                                    EmailHelper emailHelper = new EmailHelper();
                                    if (!string.IsNullOrEmpty(bookRequest.MainBooker.FirstName) && !string.IsNullOrEmpty(bookRequest.MainBooker.Email))
                                    {
                                        emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, bookRequest.MainBooker.Email, environment);
                                        emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, "claudious.neol@polussoftware.com", environment);
                                        checkSunExpressMail = true;
                                    }
                                }
                            }
                        }
                        if(bookRequest.IsReturnAvailable && checkSunExpressMail == false)
                        {
                            if (bookRequest.travelInfo.Arrival.route.Segments != null)
                            {
                                if (bookRequest.travelInfo.Arrival.route.Segments.Count() > 0)
                                {
                                    if (bookRequest.travelInfo.Arrival.route.Segments[0].Carrier.Name.ToLower() == "sunexpress")
                                    {
                                        EmailHelper emailHelper = new EmailHelper();
                                        if (!string.IsNullOrEmpty(bookRequest.MainBooker.FirstName) && !string.IsNullOrEmpty(bookRequest.MainBooker.Email)) {
                                            emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, bookRequest.MainBooker.Email, environment);
                                            emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, "claudious.neol@polussoftware.com", environment);
                                        }

                                    }
                                }
                            }
                            
                        }
                        
                        //revert bookRequest to payment url request
                        SaveBooking(bookResponse);
                        bookRequest = JsonConvert.DeserializeObject<BookRequest>(transactionDetail.BookRequest);
                        bookResponse.Request = bookRequest;
                        LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(bookResponse), "12");

                        if (bookResponse.Request.SelectedServicePackage == 3 || bookResponse.Request.SelectedServicePackage == 2)
                        {
                            SendMessage(bookResponse);
                        }

                        //Update transaction Table
                        try
                        {
                            transactionDetail.Status = "Completed";
                            _repository.PaymentTransactionRepository.UpdatePaymentTransaction(transactionDetail);
                            LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(transactionDetail), "13");

                            if (booking != null)
                            {
                                SentCreditReportAfterBooking(booking);
                            }
                            else
                            {
                                var emailExtra = bookRequest.UserEmail;
                                var bookingExtra = _repository.BookingRepository.GetBooking(c => c.UserEmail == emailExtra);
                                var bookingExtra1 = bookingExtra.OrderByDescending(x => x.CreationTime).FirstOrDefault();
                                if (bookingExtra1 != null)
                                {
                                    SentCreditReportAfterBooking(bookingExtra1);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            LogToFile.writeBookingLog("Error updating transaction : " + transactionDetail.TransactionId);
                            LogToFile.SpecficPaymentUrl("Error updating transaction : " + transactionDetail.TransactionId, "13+");

                        }

                        return Json(new Response { Data = bookResponse, Message = "success", Status = true });
                    }
                    else
                    {                                              
                        return Json(new Response { Data = bookResponse, Message = "error", Status = true });
                    }
                }
                else
                {
                    if (departureBookResponse == null || departureBookResponse.HasProblem)
                    {

                        errorMailRequest.TravelDate = bookResponse.Request.DepartureBookingObject.TravelDate;
                        errorMailRequest.IsErrorinDeparture = true;
                        SendErrorMail(errorMailRequest);
                        return Json(new Response { Data = bookResponse, Message = "error", Status = true });                           
                    }
                    else
                    {
                        if (bookRequest.IsInsuranceSelected)
                        {
                            bookResponse.DepartureInsurancePolicyNumber = BookInsurance(bookRequest.DepartureBookingObject, bookRequest, departureBookResponse);
                        }
                        if (bookRequest.IsParkingSelected)
                        {
                            bookResponse.ParkingPNRList = BookParking(bookRequest, bookResponse.DepartureBookResponse.Pnr, bookRequest.parkingSelectedCount);
                        }

                        if (bookRequest.travelInfo.Departure.route.Segments != null)
                        {
                            if (bookRequest.travelInfo.Departure.route.Segments.Count() > 0)
                            {
                                if (bookRequest.travelInfo.Departure.route.Segments[0].Carrier.Name.ToLower() == "sunexpress")
                                {
                                    EmailHelper emailHelper = new EmailHelper();
                                    if (!string.IsNullOrEmpty(bookRequest.MainBooker.FirstName) && !string.IsNullOrEmpty(bookRequest.MainBooker.Email))
                                    {
                                        emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, bookRequest.MainBooker.Email, environment);
                                        emailHelper.SubscriptionMailForSunExpress(bookRequest.MainBooker.FirstName, "claudious.neol@polussoftware.com", environment);
                                    }
                                }
                            }
                        }
                        //revert bookRequest to payment url request
                        SaveBooking(bookResponse);
                        bookRequest = JsonConvert.DeserializeObject<BookRequest>(transactionDetail.BookRequest);
                        bookResponse.Request = bookRequest;                       
                        if (bookResponse.Request.SelectedServicePackage == 3 || bookResponse.Request.SelectedServicePackage == 2)
                        {
                            SendMessage(bookResponse);
                        }

                        //Update transaction Table
                        try
                        {
                            transactionDetail.Status = "Completed";
                            _repository.PaymentTransactionRepository.UpdatePaymentTransaction(transactionDetail);

                            if(booking != null)
                            {
                                SentCreditReportAfterBooking(booking);
                            }
                            else
                            {
                                var emailExtra = bookRequest.UserEmail;
                                var bookingExtra = _repository.BookingRepository.GetBooking(c => c.UserEmail == emailExtra);
                                var bookingExtra1 = bookingExtra.OrderByDescending(x => x.CreationTime).FirstOrDefault();
                                if(bookingExtra1 != null)
                                {
                                    SentCreditReportAfterBooking(bookingExtra1);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogToFile.writeBookingLog("Error updating transaction : " + transactionDetail.TransactionId);
                        }

                        //create user account and send credentials
                        var user = _repository.Profile.GetProfiles(c => c.Email == bookRequest.MainBooker.Email);

                        return Json(new Response { Data = bookResponse, Message = "success", Status = true });
                    }                   
                }
            }
            catch (Exception ex)
            {
                LogToFile.writeBookingLog(ex.Message);
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        [Route("/book/recap")]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult BookRecap([FromBody]BookRecapRequest bookRecapRequest)
        {
            try
            {
                BookRecapAPIResponse bookRecapAPIResponse = new BookRecapAPIResponse();
                BookRecapResponse bookRecapResponse = new BookRecapResponse();
                BookRecapAPIRequest bookRecapAPIRequest = new BookRecapAPIRequest();
                bookRecapAPIRequest.Pnr = bookRecapRequest.Pnr;
                //string response = new ApiRequestHelper().GetData("/api/booking/detail", bookRecapAPIRequest).Result;
                //bookRecapAPIResponse = JsonConvert.DeserializeObject<BookRecapAPIResponse>(response);
                //bookRecapResponse.Amount = bookRecapAPIResponse.TotalSalesAmount.ToString();
                //bookRecapResponse.PaidAmount = bookRecapAPIResponse.TotalSalesAmount.ToString();
                ////bookRecapResponse.FirstName = bookRecapAPIResponse.RelativeInfo.Name;
                ////bookRecapResponse.LastName = bookRecapAPIResponse.RelativeInfo.Surname;
                //bookRecapResponse.Pnr = bookRecapRequest.Pnr;
                //bookRecapResponse.BookingStatus = bookRecapAPIResponse.BookingStatus;
                ////bookRecapResponse.BookingProducts = bookRecapAPIResponse.BookingProducts;

                return Json(new Response { Data = JObject.Parse(new ApiRequestHelper().GetData("/api/booking/detail", bookRecapAPIRequest).Result), Message = "success", Status = true });
            }
            catch
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }            
        }

        public string BookInsurance([FromRoute] Book book, [FromRoute] BookRequest bookRequest, [FromRoute] FinalBookResponse bookResponse)
        {
            try
            {
                //#region Testing

                //return String.Empty;

                //#endregion

                string insuranceBookResult = String.Empty;
                insuranceBookResult = new InsuranceHelper().BookCancellationInsurance(Math.Round(Convert.ToDecimal(book.SalesAmount), 0), bookResponse.Pnr, book.TravelDate, book.TravelDate, new InsuranceHelper().GeneratePassengerInfoForCancellationBooking(bookRequest.MainBooker, bookRequest.Passengers));
                if (insuranceBookResult == "error")
                {
                    try
                    {
                        var builder = new StringBuilder();

                        using (var reader = System.IO.File.OpenText(this.environment.WebRootPath + "\\template\\error.html"))
                        {
                            builder.Append(reader.ReadToEnd());
                        }

                        builder.Replace("{{0}}", "Insurance");
                        builder.Replace("{{1}}", bookRequest.MainBooker.FirstName + " " + bookRequest.MainBooker.LastName);
                        builder.Replace("{{2}}", bookRequest.MainBooker.Email);
                        builder.Replace("{{3}}", bookRequest.MainBooker.Phone);
                        builder.Replace("{{4}}", bookResponse.Pnr);
                        new EmailHelper().SendEmail("Insurance booking error", builder.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogToFile.writeLog("Error sending insurance mail" + bookResponse.Pnr);
                    }
                    return string.Empty;
                }
                else
                {
                    return insuranceBookResult;
                }                
            }
            catch (Exception ex)
            {
                return string.Empty;
            }            
        }

        public List<string> BookParking(BookRequest bookRequest, string PNR, int parkingSelectedCount)
        {
            try
            {
                List<ParkingResponse> parkingResponses = new List<ParkingResponse>();
                List<String> parkingPNRList = new List<string>();
                LogToFile.ParkingLogRes(JsonConvert.SerializeObject(bookRequest));
                for (int i = 0; i < parkingSelectedCount; i++)
                {
                    ParkingResponse parkingResponse = new ParkingResponse();
                    ParkingBookRequest parkingBookRequest = new ParkingBookRequest();
                    parkingBookRequest.customer = new Models.Customer();
                    parkingBookRequest.customer.email = bookRequest.DepartureBookingObject.contactInformation.Email;
                    parkingBookRequest.customer.firstName = bookRequest.DepartureBookingObject.contactInformation.FirstName;
                    parkingBookRequest.customer.flightNumber = bookRequest.DepartureBookingObject.BasketKey;
                    parkingBookRequest.customer.lastName = bookRequest.DepartureBookingObject.contactInformation.LastName;
                    parkingBookRequest.customer.mobileNumber = bookRequest.DepartureBookingObject.contactInformation.Phone;
                    parkingBookRequest.customer.numberOfPersons = bookRequest.AdultsCount + bookRequest.ChildCount + bookRequest.InfantCount;
                    parkingBookRequest.couponCode = null;
                    parkingBookRequest.currencyCode = "EUR";

                    var format = "yyyy-MM-ddTHH:mm:ssK";
                    var enterParking = Convert.ToDateTime(bookRequest.ParkingQuoteRequest.dateTimeFrom);
                    var newEnter = new DateTime(enterParking.Year, enterParking.Month, enterParking.Day, enterParking.Hour, enterParking.Minute, enterParking.Second, DateTimeKind.Local);
                    parkingBookRequest.enterDateTime = newEnter.ToString(format);
                    //parkingBookRequest.enterDateTime = bookRequest.ParkingQuoteRequest.dateTimeFrom;


                    var enterParkingArrival = Convert.ToDateTime(bookRequest.ParkingQuoteRequest.dateTimeTo);
                    var newEnterArrival = new DateTime(enterParkingArrival.Year, enterParkingArrival.Month, enterParkingArrival.Day, enterParkingArrival.Hour, enterParkingArrival.Minute, enterParkingArrival.Second, DateTimeKind.Local);
                    parkingBookRequest.exitDateTime = newEnterArrival.ToString(format);
                    //parkingBookRequest.exitDateTime = bookRequest.ParkingQuoteRequest.dateTimeTo;
                    parkingBookRequest.enableFlightTimeCalculation = "true";

                    parkingBookRequest.externalReference = PNR;
                    parkingBookRequest.labelAbbreviation = bookRequest.ParkingQuoteRequest.labelAbbreviation;
                    parkingBookRequest.locationAbbreviation = bookRequest.ParkingQuoteRequest.locationAbbreviation;
                    parkingBookRequest.paymentLocation = "baCharged";
                    parkingBookRequest.posCode = "";
                    parkingBookRequest.productAbbreviation = bookRequest.ParkingSelectedAbbreviation;
                    parkingBookRequest.referral = "";
                    parkingBookRequest.externalReference = PNR;
                    parkingResponse = JsonConvert.DeserializeObject<ParkingResponse>(new ApiRequestHelper().PostData(Utils.Constants.ParkingURL + "bookings", JsonConvert.SerializeObject(parkingBookRequest)).Result);
                    parkingResponses.Add(parkingResponse);
                    parkingPNRList.Add(parkingResponse.code);
                }                
                return parkingPNRList;
            }
            catch (Exception)
            {
                var builder = new StringBuilder();

                using (var reader = System.IO.File.OpenText(this.environment.WebRootPath + "\\template\\error.html"))
                {
                    builder.Append(reader.ReadToEnd());
                }

                builder.Replace("{{0}}", "Parking");
                builder.Replace("{{1}}", bookRequest.MainBooker.FirstName + " " + bookRequest.MainBooker.LastName);
                builder.Replace("{{2}}", bookRequest.MainBooker.Email);
                builder.Replace("{{3}}", bookRequest.MainBooker.Phone);
                builder.Replace("{{4}}", PNR);
                new EmailHelper().SendEmail("Parking booking error", builder.ToString());

                return new List<string>();
            }            
        }

        public bool AddExtrenalItem(string basketKey, string explain, double amount, string PriceExplain)
        {
            try
            {               
                ExternalItemRequest externalItemRequest = new ExternalItemRequest();
                externalItemRequest.BasketKey = basketKey;
                externalItemRequest.CurrencyCode = "EUR";
                externalItemRequest.Items = new Item[1];
                externalItemRequest.Items[0] = new Item();               
                externalItemRequest.Items[0].ItemCount = 1;
                externalItemRequest.Items[0].Prices = new Price[1];
                externalItemRequest.Items[0].Prices[0] = new Price();
                externalItemRequest.Items[0].Explain = explain;
                externalItemRequest.Items[0].Prices[0].PriceExplain = PriceExplain;
                externalItemRequest.Items[0].Prices[0].Amount = amount;
                if (explain.ToLower() == "max")
                {
                    externalItemRequest.Items[0].Explain = "Max - Servicepakket";
                    externalItemRequest.Items[0].Prices[0].PriceExplain = "Max - Servicepakket";
                }
                _ = new ApiRequestHelper().GetData("/api/basket/externalitems/create", externalItemRequest).Result;
                return true;
            }
            catch (Exception ex)
            {
                LogToFile.writeBookingLog("AddExtrenalItem" + ex.Message);
                return false;
            }
        }

        public string Prebook(string basketKey, Book book)
        {
            try
            {
                PrebookRequest prebookRequest = new PrebookRequest();
                PrebookResponse prebookResponse = new PrebookResponse();
                prebookRequest.BasketKey = basketKey;
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(prebookRequest), "9");

                string response = new ApiRequestHelper().GetData("/api/airpool/prebook", prebookRequest).Result;
                prebookResponse = JsonConvert.DeserializeObject<PrebookResponse>(response);
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(book), "10");

                return new ApiRequestHelper().GetData("/api/booking/create", book).Result;
            }
            catch (Exception ex)
            {
                LogToFile.writeBookingLog("Book API: " + ex.Message);
                return "error";
            }
        }

        [HttpPost("/applypromocode")]
        [AllowAnonymous]
        public JsonResult ApplyPromoCode(PromoCodeRequestViewModel promoCodeRequestViewModel)
        {
            try
            {
                if (promoCodeRequestViewModel.CouponCode == Constants.SuvendusCouponCode)
                {
                    //check already applied a suvendus coupon
                    var bookings = _repository.BookingRepository.GetBooking(c => c.UserEmail == promoCodeRequestViewModel.Email);
                    if (bookings != null && bookings.Count() > 0)
                    { 
                        foreach(var booking in bookings)
                        {
                            if (booking.SuvendusApplied == "YES")
                            {
                                return Json(new Response { Data = new PromotionResponse { IsOk = false, Message = ""}, Message = "success", Status = true });
                            }
                        }
                    }

                    return Json(new Response { Data = new PromotionResponse { IsOk = true, Message = "" }, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = new PromotionResponse { IsOk = false, Message = "" }, Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = new PromotionResponse { IsOk = false, Message = "" }, Message = "error", Status = false });
            }
            //try
            //{
            //    PromotionResponse promotionResponse = new PromotionResponse();
            //    promotionResponse = ApplyPromoAPI(promoCodeRequestViewModel.BasketKey, promoCodeRequestViewModel.CouponCode);
            //    if (promotionResponse.IsOk)
            //    {
            //        return Json(new Response { Data = promoCodeRequestViewModel, Message = "success", Status = true });
            //    }
            //    else
            //    {
            //        return Json(new Response { Data = promoCodeRequestViewModel, Message = "error", Status = false });
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogToFile.writeBookingLog("Promo code : " + ex.Message);
            //    return Json(new Response { Data = promoCodeRequestViewModel, Message = "error", Status = false });
            //}
        }

        [HttpGet("/applysuvenduscoupon/{coupon}")]
        [AllowAnonymous]
        public JsonResult ApplySuvendusCoupon(string coupon)
        {
            try
            {
                if (coupon == Constants.SuvendusCouponCode)
                {
                    return Json(new Response { Data = Constants.SuvendusCouponAmount, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = 0, Message = "error", Status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = string.Empty, Message = "error", Status = false });
            }
        }

        public PromotionResponse ApplyPromoAPI(string basketKey, string couponCode)
        {
            PromotionResponse promotionResponse = new PromotionResponse();
            string RequestContent = "{ 'BasketKey': '" + basketKey + "', 'LangugeCode': 'NL', 'AddThePromotionToBasket': true, 'DeleteThePromotionToBasket': null, 'PromotionCode': '" + couponCode + "' }";
            string response = new ApiRequestHelper().GetData("/api/basket/createpromotionbasket", RequestContent).Result;
            promotionResponse = JsonConvert.DeserializeObject<PromotionResponse>(response);
            return promotionResponse;
        }

        [HttpPost("/applymaxdiscount")]
        [AllowAnonymous]
        public JsonResult ApplyMaxDisocunt(ApplyMaxDiscountViewModel applyMaxDiscountViewModel)
        {
            MaxDiscountReturnViewModel maxDiscountReturnViewModel = new MaxDiscountReturnViewModel();
            try
            {
                var user = _repository.Profile.GetProfiles(c => c.Email == applyMaxDiscountViewModel.UserId).FirstOrDefault();
                var bookings = _repository.BookingRepository.GetBooking(c => c.UserId == user.Id);
                if (bookings != null && bookings.Count() > 0)
                {
                    Booking booking = bookings.OrderByDescending(c => c.Id).First();
                    BookResponse bookResponse = new BookResponse();
                    bookResponse = JsonConvert.DeserializeObject<BookResponse>(booking.BookingDetails);
                    if (bookResponse.Request.SelectedServicePackage == 3)
                    {
                        PromotionResponse promotionResponse = new PromotionResponse();
                        maxDiscountReturnViewModel.DiscountAmount = booking.ServicePackageAmount;
                        maxDiscountReturnViewModel.DiscountName = Constants.MaxPackageName;
                        maxDiscountReturnViewModel.IsApplicable = true;
                        return Json(new Response { Data = maxDiscountReturnViewModel, Message = "success", Status = true });
                    }
                }               
                return Json(new Response { Data = maxDiscountReturnViewModel, Message = "error", Status = false });
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = maxDiscountReturnViewModel, Message = "error", Status = false });
            }
        }

        public string SendMessage(BookResponse bookResponse)
        {
            try
            {
                Message message = new Message();
                MessageContent messageContent = new MessageContent();
                bool isRoundTrip = bookResponse.Request.IsReturnAvailable;
                message.recipients.Add(bookResponse.Request.MainBooker.Phone);
                message.body = new SmsHelper().GenerateSMSContent(bookResponse, isRoundTrip);
                _ = new SmsHelper().SendConfirmationAsync(message).Result;
                return "success";
            }
            catch
            {
                return "error";
            }
        }

        public bool SaveBooking(BookResponse bookResponse)
        {
            try
            {
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(bookResponse), "14");

                Booking booking = new Booking();
                booking.CustomerName = bookResponse.Request.MainBooker.FirstName + " " + bookResponse.Request.MainBooker.LastName;
                booking.DeparturePnr = bookResponse.DepartureBookResponse.Pnr;
                booking.IsRoundTrip = bookResponse.Request.IsReturnAvailable;
                booking.PaidAmount = booking.TotalAmount;
                booking.UserEmail = bookResponse.UserEmail;
                booking.SuvendusApplied = bookResponse.SuvendusApplied;
                if (booking.IsRoundTrip)
                {
                    booking.ReturnPnr = bookResponse.ReturnBookResponse.Pnr;
                }
                else 
                {
                    booking.ReturnPnr = String.Empty;
                }
                if (bookResponse.Request.SelectedServicePackage == 3)
                {
                    booking.ServicePackageAmount = Constants.MaxPackageAmount;
                }
                booking.TotalAmount = bookResponse.Request.TotalAmount;
                booking.UserId = RegisterUser(bookResponse.Request);
                booking.BookingDetails = JsonConvert.SerializeObject(bookResponse);
                booking.CreditApplied = bookResponse.Request.AppliedCredit;
                booking.CreditReceived = Convert.ToInt32(bookResponse.Request.TotalAmount);
                _repository.BookingRepository.CreateBooking(booking);
                if (booking.Id == 0)
                {
                    LogToFile.writeLog("Error saving booking to DB : " + JsonConvert.SerializeObject(bookResponse));
                }
                if (booking.IsRoundTrip)
                {
                    _ = SavePaymentAsync(bookResponse.ReturnBookResponse.Pnr, (bookResponse.Request.TotalAmount - Convert.ToDecimal(bookResponse.Request.DepartureBookingObject.SalesAmount)), bookResponse.Request.MainBooker.Email, booking.Id).Result;
                }
                _ = SavePaymentAsync(bookResponse.DepartureBookResponse.Pnr, Convert.ToDecimal(bookResponse.Request.DepartureBookingObject.SalesAmount), bookResponse.Request.MainBooker.Email, booking.Id).Result;

                return true;
            }
            catch (Exception ex)
            {
                LogToFile.writeBookingLog("Error saving booking to DB : " + JsonConvert.SerializeObject(bookResponse));
                LogToFile.writeBookingLog("Error saving booking to DB. Exception : " + ex.Message);
                return false;
            }
        }
        
        public int RegisterUser(BookRequest bookRequest)
        {
            var profile = _repository.Profile.GetProfiles(c => c.Email == bookRequest.UserEmail).FirstOrDefault();
            if (profile != null)
            {
                return profile.Id;
            }
            else
            {
                Profile newProfile = new Profile();
                newProfile.DOB = DateTime.Parse(bookRequest.MainBooker.BirthDate.Substring(0, 4)
                    + "-" + bookRequest.MainBooker.BirthDate.Substring(4, 2)
                    + "-" + bookRequest.MainBooker.BirthDate.Substring(6, 2));
                newProfile.Email = bookRequest.MainBooker.Email;
                newProfile.IsActive = true;
                newProfile.IsVerified = true;
                newProfile.Mobile = bookRequest.MainBooker.Phone;
                newProfile.Name = bookRequest.MainBooker.FirstName;
                newProfile.Surname = bookRequest.MainBooker.LastName;
                newProfile.Password = RandomPassword();
                _repository.Profile.CreateProfile(newProfile);

                try
                {
                    TextPart data;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<h3>Welkom als lid bij Ticketvoordeel.nl</h3>");
                    sb.Append("<div>Beste " + newProfile.Name + ",</div>");
                    sb.Append("<div>Allereerst bedankt voor uw leuke boeking.</div>");
                    sb.Append("<div>Hieronder treft u uw gebruikersnaam en wachtwoord aan voor uw Ticketvoordeel.nl account.</div>");
                    sb.Append("<div>U kunt met de onderstaande gegevens inloggen via 'Mijn Boeking' via www.ticketvoordeel.nl</div>");
                    sb.Append("<div>Uw gebruikersnaam : " + newProfile.Email + "</div>");
                    sb.Append("<div>Uw wachtwoord : " + newProfile.Password + "</div>");
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

                    new EmailHelper().SendMail(newProfile.Email, "Welkom als lid bij Ticketvoordeel.nl", data);
                }
                catch (Exception ex)
                {

                }

                return newProfile.Id;
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
                var webRoot = environment.WebRootPath;
                var PathWithFolderName = Path.Combine(webRoot, "errorlogs");
                Guid guid = Guid.NewGuid();
                string filename = guid.ToString() + ".txt";
                using (StreamWriter writer = System.IO.File.CreateText(PathWithFolderName + "\\" + filename))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(errorMailRequest));
                }
                sbMailContent.Append("<div><b>Send this filename to the developer for more info: </b>" + filename + "</div>");
            }
            catch(Exception ex)
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

        public Book PopulateAddress([FromQuery]Book book, [FromQuery]MainBooker mainBooker)
        {
            book.contactInformation.PostCode = mainBooker.PostCode;
            book.contactInformation.City = mainBooker.Place;
            book.contactInformation.AddressLine = mainBooker.HouseNumber + " " + mainBooker.Street;
            book.CustomerInformation = new ContactInformation();
            book.CustomerInformation = book.contactInformation;

            return book;
        }

        //public BookingDetail GetBookingDetail(string pnr)
        //{
        //    try
        //    {
        //        BookingDetailRequest bookingDetailRequest = new BookingDetailRequest();
        //        BookingDetail bookingDetail = new BookingDetail();

        //        bookingDetailRequest.Pnr = pnr;
        //        bookingDetailRequest.Section = 31;
        //        string response = new ApiRequestHelper().GetData("/api/booking/detail", bookingDetailRequest).Result;
        //        bookingDetail = JsonConvert.DeserializeObject<BookingDetail>(response);
        //        return bookingDetail;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BookingDetail();
        //    }           
        //}

        #region Payment

        public async Task<bool> SavePaymentAsync(string pnr, decimal amount, string email, int bookingId)
        {
            try
            {
                //Payment to Tursys
                PaymentCollection paymentCollection = new PaymentCollection();
                paymentCollection.BookingPnr = pnr;
                paymentCollection.PaymentAmount = amount;
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(paymentCollection), "15");

                _ = new ApiRequestHelper().GetData("/api/account/booking/collectingpayment", paymentCollection).Result;

                //Send automatic invoice
                TicketRequest ticketRequest = new TicketRequest();
                ticketRequest.pnr = pnr;
                ticketRequest.toEmail = email;
                ticketRequest.subject = "Bedankt voor uw betaling";
                ticketRequest.type = 1;
                SendInvoiceAndTicketAsync(ticketRequest);
                TicketRequest ticketRequestTicket = new TicketRequest();
                ticketRequest.pnr = pnr;
                ticketRequest.toEmail = email;
                ticketRequest.subject = "Travel Ticket";
                ticketRequest.type = 2;
                LogToFile.SpecficPaymentUrl(JsonConvert.SerializeObject(ticketRequestTicket), "16");

                SendInvoiceAndTicketAsync(ticketRequestTicket);
                return true;

                //try
                //{
                //    LogToFile.writePaymentLog("Payment started :" + trxid);
                //}
                //catch (Exception ex)
                //{
                //    LogToFile.writePaymentLog(ex.Message);
                //}

                //SisowPaymentGateway sisowPaymentGateway = new SisowPaymentGateway();
                //sisowPaymentGateway = await FindPaymentInfoByStatusRequest(trxid);

                //if (sisowPaymentGateway.status == "Expired" && sisowPaymentGateway.status == "Cancelled" && sisowPaymentGateway.status == "Failure" && sisowPaymentGateway.status == "Reversed" && sisowPaymentGateway.status == "Denied")
                //{
                //    LogToFile.writePaymentLog("Payment Failure: " + sisowPaymentGateway.status);
                //    return false;
                //}
                //if (sisowPaymentGateway.status == "Success")
                //{
                //    PaymentHistory payment = new PaymentHistory();
                //    if (sisowPaymentGateway.entranceCode == "paypalec")
                //    {
                //        payment.Amount = Math.Round(((sisowPaymentGateway.amount * 100) / 102.5), 2).ToString();
                //    }
                //    else
                //    {
                //        payment.Amount = sisowPaymentGateway.amount.ToString();
                //    }
                //    payment.Description = sisowPaymentGateway.description;
                //    payment.EntanceCode = sisowPaymentGateway.entranceCode;
                //    payment.IssuerId = sisowPaymentGateway.issuerId;
                //    payment.PaymentId = sisowPaymentGateway.payment;
                //    payment.PurchaseId = sisowPaymentGateway.purchaseId;
                //    payment.ShopId = sisowPaymentGateway.shopId;
                //    payment.Status = sisowPaymentGateway.status;
                //    payment.TestMode = sisowPaymentGateway.testMode;
                //    payment.TransactionId = trxid;
                //    payment.BookingId = bookingId;
                //    payment.PaidDate = System.DateTime.Now;

                //    try
                //    {
                //        LogToFile.writePaymentLog("Writing to DB"); ;
                //    }
                //    catch (Exception ex)
                //    {
                //        LogToFile.writePaymentLog(ex.Message);
                //    }

                //    try
                //    {
                //        PaymentHistory existingPayment = new PaymentHistory();
                //        existingPayment = _repository.PaymentRepository.GetPayment(c => (c.Description == sisowPaymentGateway.description && c.PurchaseId == sisowPaymentGateway.purchaseId)).FirstOrDefault();
                //        if (existingPayment != null)
                //        {
                //            if (existingPayment.Id > 0)
                //            {
                //                LogToFile.writePaymentLog("Payment already exist");
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        LogToFile.writePaymentLog("Error checking existing payment");
                //    }

                //    try
                //    {
                //        _repository.PaymentRepository.CreatePayment(payment);
                //    }
                //    catch (Exception ex)
                //    {
                //        LogToFile.writePaymentLog("Error saving payment to db");
                //    }                    

                //    //Payment to Tursys
                //    string packageRequestContent = "{ 'BookingPnr': '" + pnr + "', 'PosCode': 'SisowMC', 'OrderNo': 'NA', 'Remark': 'NA', 'TransactionType': 1, 'CurrencyCode': 'EUR','AccountOwner': 'NA', 'PaymentAmount': " + amount + " }";
                //    _ = new ApiRequestHelper().GetData("/api/account/booking/collectingpayment", packageRequestContent).Result;

                //    //Send automatic invoice
                //    TicketRequest ticketRequest = new TicketRequest();
                //    ticketRequest.pnr = pnr;
                //    ticketRequest.toEmail = email;
                //    ticketRequest.subject = "Bedankt voor uw betaling";
                //    ticketRequest.type = 1;
                //    SendInvoiceAndTicketAsync(ticketRequest);
                //    return true;

                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                LogToFile.writeLog("Error saving payment to DB : trx - ,pnr - " + pnr);
                LogToFile.writeLog("Error saving payment to DB. Exception : " + ex.Message);
                return false;
            }
        }

        public async Task<ActionResult> Error(string trxid, bool bookingError = false)
        {
            return Json(new Response { Data = string.Empty, Message = "error", Status = false });
        }

        public async Task<string> FindPaymentStatus(string trxid)
        {
            try
            {
                SisowPaymentGateway sisowPaymentGateway = new SisowPaymentGateway();
                await sisowPaymentGateway.StatusRequest(trxid);
                return sisowPaymentGateway.status;
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public async Task<SisowPaymentGateway> FindPaymentInfoByStatusRequest(string trxid)
        {
            try
            {
                SisowPaymentGateway sisowPaymentGateway = new SisowPaymentGateway();
                await sisowPaymentGateway.StatusRequest(trxid);
                return sisowPaymentGateway;
            }
            catch (Exception ex)
            {
                return new SisowPaymentGateway();
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

        [HttpPost("/sendticket")]
        [AllowAnonymous]
        public JsonResult SendInvoiceAndTicketAsync(TicketRequest ticketRequest)
        {
            try
            {
                if (ticketRequest.pnr != null)
                {
                    if (ticketRequest.type == 2)
                    {
                        ticketRequest.type = 4;
                    }

                    InvoiceRequest invoiceRequest = new InvoiceRequest();
                    invoiceRequest.ExtraRemarks = "";
                    invoiceRequest.IsPDF = true;
                    invoiceRequest.Pnr = ticketRequest.pnr;
                    //invoiceRequest.PriceType = 1;
                    invoiceRequest.RelationId = "0";
                    invoiceRequest.Subject = ticketRequest.subject;
                    invoiceRequest.ToEmail = ticketRequest.toEmail;
                    invoiceRequest.Type = ticketRequest.type;

                    _ = new ApiRequestHelper().GetData("/api/output/sendemail", invoiceRequest).Result;

                    return Json(new Response { Data = invoiceRequest, Message = "success", Status = true });
                }
                else
                {
                    return Json(new Response { Data = String.Empty, Message = "success", Status = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new Response { Data = String.Empty, Message = "error", Status = false });
            }
        }

        public void Pay(string pnr, decimal amount)
        {
            PaymentCollection paymentCollection = new PaymentCollection();
            paymentCollection.BookingPnr = pnr;
            paymentCollection.PaymentAmount = amount;
            //string paymentRequest = "{ 'BookingPnr': '" + pnr + "', 'PosCode': 'SisowMC', 'OrderNo': 'NA', 'Remark': 'NA', 'TransactionType': 1, 'CurrencyCode': 'EUR','AccountOwner': 'NA', 'PaymentAmount': " + amount + " }";
            _ = new ApiRequestHelper().GetData("/api/account/booking/collectingpayment", paymentCollection).Result;
        }

        public MainBooker ConvertMainBookerCharacters(MainBooker mainBooker)
        {
            MainBooker updatedBooker = new MainBooker();
            updatedBooker.Email = new StringFormatterHelper().RemoveDiacritics(mainBooker.Email);
            updatedBooker.FirstName = new StringFormatterHelper().RemoveDiacritics(mainBooker.FirstName).Trim();
            updatedBooker.HouseNumber = new StringFormatterHelper().RemoveDiacritics(mainBooker.HouseNumber).Trim();
            updatedBooker.LastName = new StringFormatterHelper().RemoveDiacritics(mainBooker.LastName);
            updatedBooker.Phone = new StringFormatterHelper().RemoveDiacritics(mainBooker.Phone);
            updatedBooker.Place = new StringFormatterHelper().RemoveDiacritics(mainBooker.Place);
            updatedBooker.PostCode = new StringFormatterHelper().RemoveDiacritics(mainBooker.PostCode);
            updatedBooker.Street = new StringFormatterHelper().RemoveDiacritics(mainBooker.Street);
            updatedBooker.BirthDate = mainBooker.BirthDate;

            return updatedBooker;
        }

        public List<Ticketvoordeel.Models.BookRequest.Passenger> ConvertPassengerCharacters(List<Ticketvoordeel.Models.BookRequest.Passenger> passengers)
        {
            List<Ticketvoordeel.Models.BookRequest.Passenger> updatedList = new List<Models.BookRequest.Passenger>();
            if (passengers != null && passengers.Count() > 0)
            {
                foreach (var pass in passengers)
                {
                    Models.BookRequest.Passenger passenger = new Models.BookRequest.Passenger();
                    passenger = pass;
                    passenger.FirstName = new StringFormatterHelper().RemoveDiacritics(pass.FirstName).Trim();
                    passenger.LastName = new StringFormatterHelper().RemoveDiacritics(pass.LastName).Trim();

                    updatedList.Add(passenger);
                }                
            }
            return updatedList;
        }

        public List<Ticketvoordeel.Models.CreateBasket.Passenger> ConvertPassengerCharacters1(List<Ticketvoordeel.Models.CreateBasket.Passenger> passengers)
        {
            List<Ticketvoordeel.Models.CreateBasket.Passenger> updatedList = new List<Models.CreateBasket.Passenger>();
            if (passengers != null && passengers.Count() > 0)
            {
                foreach (var pass in passengers)
                {
                    Models.CreateBasket.Passenger passenger = new Models.CreateBasket.Passenger();
                    passenger = pass;
                    passenger.FirstName = new StringFormatterHelper().RemoveDiacritics(pass.FirstName).Trim();
                    passenger.LastName = new StringFormatterHelper().RemoveDiacritics(pass.LastName).Trim();

                    updatedList.Add(passenger);
                }
            }
            return updatedList;
        }

        public ContactInformation ConvertContactInformationCharacters(ContactInformation contactInformation)
        {
            ContactInformation updatedContactInformation = new ContactInformation();
            updatedContactInformation = contactInformation;
            updatedContactInformation.FirstName = new StringFormatterHelper().RemoveDiacritics(contactInformation.FirstName);
            updatedContactInformation.LastName = new StringFormatterHelper().RemoveDiacritics(contactInformation.LastName);

            return updatedContactInformation;
        }

        [HttpGet("/creditmail")]
        [AllowAnonymous]
        public void SentCreditReportAfterBooking(Booking booking)
        {
            try
            {
                if(booking != null)
                {
                    EmailHelper emailHelper = new EmailHelper();
                    var totalCredit = 0;
                    var earnedCredit = booking.CreditReceived;
                    var userName = booking.CustomerName;
                    var mailAddress = booking.UserEmail;
                    if(string.IsNullOrEmpty(mailAddress))
                    {
                        var fetchEmail = _repository.Profile.GetProfileById(booking.UserId);
                        if(fetchEmail != null)
                        {
                            mailAddress = fetchEmail.Email;
                        }
                    }
                    var usedTicks = 0;
                    var totalTicks = 0;

                    var bookingDetails = _repository.BookingRepository.GetBooking(c => c.UserId == booking.UserId);
                    foreach (var items in bookingDetails)
                    {
                        totalTicks += items.CreditReceived;
                        usedTicks += items.CreditApplied;
                    }

                    totalCredit = totalTicks - usedTicks;
                    if(totalCredit < 0)
                    {
                        totalCredit = 0;
                        earnedCredit = 0;
                    }

                    if (!string.IsNullOrEmpty(mailAddress))
                    {
                        emailHelper.CreditNotficationMail(Convert.ToString(totalCredit), Convert.ToString(earnedCredit), userName, mailAddress, environment);
                        if(mailAddress != "info@ticketvoordeel.nl")
                        {
                            emailHelper.CreditNotficationMail(Convert.ToString(totalCredit), Convert.ToString(earnedCredit), "Ticketvoordeel User: " + userName, "info@ticketvoordeel.nl", environment);
                        }
                        emailHelper.CreditNotficationMail(Convert.ToString(totalCredit), Convert.ToString(earnedCredit), userName, "noeltesting08@outlook.com", environment);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

       
        [HttpGet("/SubscribeNewsletter")]
        [AllowAnonymous]
        public void SubscribeNewsletter(string email, string name)
        {
            try
            {
                Subscription subscription = new Subscription();
                var profiles = _repository.Profile.GetAllProfiles();
                var getName = profiles.Where(c => c.Email == email).FirstOrDefault();
                if (getName != null)
                {
                    subscription.Name = name;
                }
                else
                {
                    subscription.Name = name;
                }
                subscription.Email = email;
                subscription.CreationTime = DateTime.Now;
                subscription.IsActive = "true";

                subscription = _repository.Subscription.CreateSubscription(subscription);
                EmailHelper emailHelper = new EmailHelper();
                emailHelper.SubscriptionMailNotify(subscription.Name, email, environment);
                emailHelper.SubscriptionMailNotifyForTV(subscription.Name, email, environment);
            }
            catch (Exception ex)
            {

            }
        }

        [HttpPost("/creditticksformanuelbooking")]
        [AllowAnonymous]
        public void CreditTicksForManualBooking(Booking newBooking)
        {
            {
                Booking booking = new Booking();
                BookResponse bookResponse = new BookResponse();
                booking.CustomerName = "Noel";
                booking.DeparturePnr = "ZZZZZZ";
                booking.IsRoundTrip = false;
                booking.PaidAmount = 150.00M;
                booking.UserEmail = "noelmanuel.n8@gmail.com";
                booking.SuvendusApplied = null;
                if (booking.IsRoundTrip)
                {
                    booking.ReturnPnr = "ZZZZZZ";
                }
                else
                {
                    booking.ReturnPnr = String.Empty;
                }

                booking.ServicePackageAmount = 0;

                booking.TotalAmount = 150.00M;
                booking.UserId = RegisterNewUser("noelmanuel.n8@gmail.com");
                booking.BookingDetails = JsonConvert.SerializeObject(bookResponse);
                booking.CreditApplied = 0;
                booking.CreditReceived = 150;
                _repository.BookingRepository.CreateBooking(booking);
            }

        }

        public int RegisterNewUser(string UserEmail)
        {
            var profile = _repository.Profile.GetProfiles(c => c.Email == UserEmail).FirstOrDefault();
            if (profile != null)
            {
                return profile.Id;
            }
            else
            {
                Profile newProfile = new Profile();
                newProfile.DOB = DateTime.Now;
                newProfile.Email = UserEmail;
                newProfile.IsActive = true;
                newProfile.IsVerified = true;
                newProfile.Mobile = "9809238820";
                newProfile.Name = "Noel";
                newProfile.Surname = "Manuel";
                newProfile.Password = RandomPassword();
                _repository.Profile.CreateProfile(newProfile);

                return newProfile.Id;
            }
        }

        #endregion
    }
}