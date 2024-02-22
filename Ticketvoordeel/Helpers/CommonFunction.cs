using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.AirpoolSearchResponse;

namespace Ticketvoordeel.Helpers
{
    public class CommonFunction
    {
        public void FetchAndStoreLastMinuteDetail(List<LastMinuteDeal> lastMinuteDealsList, IRepositoryWrapper _repository)
        {
            var minDateInLastMinuteList = lastMinuteDealsList.Count > 0 ? lastMinuteDealsList.Min(cv => cv.UpdatedTime) : DateTime.Now;

            if (minDateInLastMinuteList != DateTime.MinValue && minDateInLastMinuteList <= DateTime.Now)
            {
                var getAllLastMinDetails = _repository.LastMinuteDeals.GetAllLastMinuteDeals();
                foreach (var eachMinOrder in getAllLastMinDetails)
                {
                    _repository.LastMinuteDeals.DeleteLastMinuteDeal(eachMinOrder);
                }

                #region SETTING DATE
                var noOfDayInLoop = 5;
                    var getNextMonth = DateTime.Now.AddMonths(1).Month;
                    var formattedNextMonth = getNextMonth.ToString();
                    if (formattedNextMonth.Length == 1)
                    {
                        formattedNextMonth = "0" + formattedNextMonth;
                    }
                    var getNextYear = DateTime.Now.Year.ToString();
                    var getNextDay = "05";
                #endregion

                #region SETTING LOCATION
                string[] departureLocationIstanbul = { "AMS" };
                string[] destinationLocationIstanbul = { "DUS", "SAW", "IST" };

                string[] departureLocationIzmir = { "AMS", "ADB" };
                string[] destinationLocationIzmir = { "ADB", "AMS" };

                string[] departureLocationKayseri = { "AMS", "DUS", "ASR" };
                string[] destinationLocationKayseri = { "AMS", "ASR" };
                #endregion

                #region ISTANBUL
                List<LastMinuteDeal> lastMinuteDeals = new List<LastMinuteDeal>();
                var cout = 0;

                for (var i = 0; i <= noOfDayInLoop; i++)
                {
                    var departureDate = getNextYear + "-" + formattedNextMonth + "-" + (Convert.ToInt32(getNextDay) + i);

                    foreach (var dept in departureLocationIstanbul)
                    {
                        foreach (var dest in destinationLocationIstanbul)
                        {
                            if (dept != dest)
                            {
                                var airpoolSearchResponse = ProcessFlightSearch(dept, dest, departureDate);

                                if (airpoolSearchResponse != null)
                                {
                                    foreach (var items in airpoolSearchResponse?.Reader?.FareGroups)
                                    {
                                        foreach (var route in items.Routes)
                                        {
                                            if (destinationLocationIstanbul.Contains(route[0].DestinationAirport.Code) && departureLocationIstanbul.Contains(route[0].OriginAirport.Code))
                                            {
                                                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();

                                                Passengers passengers = new Passengers();

                                                passengers.Adult = 1;
                                                passengers.Child = 0;
                                                passengers.Infant = 0;


                                                Models.Route route1 = new Models.Route()
                                                {
                                                    Origin = new Origin()
                                                    {
                                                        Code = dept,
                                                        IsCity = false,
                                                    },
                                                    Destination = new Destination()
                                                    {
                                                        Code = dest,
                                                        IsCity = false,
                                                    },
                                                    Departure = new Departure()
                                                    {
                                                        Date = departureDate,
                                                        DaysBefore = 1,
                                                        DaysAfter = 0,
                                                    },
                                                    FlightType = new FlightType()
                                                    {
                                                        MaxConnections = 0,
                                                        ConnectionType = 4,
                                                    },
                                                };

                                                PoolRequestBaseViewModel root = new PoolRequestBaseViewModel();

                                                PoolRequest ff = new PoolRequest();
                                                ff.Routes = new List<Models.Route>();
                                                ff.Routes.Add(route1);
                                                ff.Passengers = passengers;


                                                root.PoolRequest = ff;
                                                root.CurrencyCode = "EUR";

                                                var NewPool = JsonConvert.SerializeObject(root);

                                                lastMinuteDeal.PoolSearchRequest = NewPool;
                                                lastMinuteDeal.Title = "Istanbul";
                                                lastMinuteDeal.Price = route[0].TotalPrice.Total;
                                                lastMinuteDeal.Airline = route[0].MarketingCarrier.Name;
                                                lastMinuteDeal.DepartureTime = FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Minute);
                                                lastMinuteDeal.ArrivalTime = FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Minute);
                                                lastMinuteDeal.ToLocation = dest;
                                                lastMinuteDeal.FromLocation = dept;
                                                lastMinuteDeal.Id = cout;
                                                lastMinuteDeals.Add(lastMinuteDeal);
                                                cout++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var OrderedListForIstanbul = lastMinuteDeals.OrderBy(x => x.Price);
                int counter = 0;
                foreach (var pickItem in OrderedListForIstanbul)
                {
                    if (counter <= 2)
                    {
                        LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();
                        lastMinuteDeal.PoolSearchRequest = pickItem.PoolSearchRequest;
                        lastMinuteDeal.Title = pickItem.Title;
                        lastMinuteDeal.Price = pickItem.Price;
                        lastMinuteDeal.Airline = pickItem.Airline;
                        lastMinuteDeal.DepartureTime = pickItem.DepartureTime;
                        lastMinuteDeal.ArrivalTime = pickItem.ArrivalTime;
                        lastMinuteDeal.ToLocation = pickItem.ToLocation;
                        lastMinuteDeal.FromLocation = pickItem.FromLocation;
                        lastMinuteDeal.UpdatedTime = DateTime.Now;
                        lastMinuteDeal.CreationTime = DateTime.Now;
                        lastMinuteDeal.IsActive = true;
                        lastMinuteDeal.ColumnNumber = 1;

                        _repository.LastMinuteDeals.UpdateLastMinuteDeal(lastMinuteDeal);
                    }
                    else
                    {
                        break;
                    }
                    counter++;
                }
                #endregion

                #region IZMIR
                List<LastMinuteDeal> lastMinuteDealsIzmir = new List<LastMinuteDeal>();
                var coutIzmir = 0;

                for (var i = 0; i <= noOfDayInLoop; i++)
                {
                    var departureDate = getNextYear + "-" + formattedNextMonth + "-" + (Convert.ToInt32(getNextDay) + i);
                                       
                    foreach (var dept in departureLocationIzmir)
                    {
                        foreach (var dest in destinationLocationIzmir)
                        {
                            if (dept != dest)
                            {
                                var airpoolSearchResponse = ProcessFlightSearch(dept, dest, departureDate);

                                if (airpoolSearchResponse != null)
                                {
                                    foreach (var items in airpoolSearchResponse?.Reader?.FareGroups)
                                    {
                                        foreach (var route in items.Routes)
                                        {
                                            if (destinationLocationIzmir.Contains(route[0].DestinationAirport.Code) && departureLocationIzmir.Contains(route[0].OriginAirport.Code))
                                            {
                                                if (Convert.ToInt32(getNextDay) + i == 30)
                                                {

                                                }
                                                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();

                                                Passengers passengers = new Passengers();

                                                passengers.Adult = 1;
                                                passengers.Child = 0;
                                                passengers.Infant = 0;
                                                

                                                Models.Route route1 = new Models.Route()
                                                {
                                                    Origin = new Origin()
                                                    {
                                                        Code = dept,
                                                        IsCity = false,
                                                    },
                                                    Destination = new Destination()
                                                    {
                                                        Code = dest,
                                                        IsCity = false,
                                                    },
                                                    Departure = new Departure()
                                                    {
                                                        Date = departureDate,
                                                        DaysBefore = 1,
                                                        DaysAfter = 0,
                                                    },
                                                    FlightType = new FlightType()
                                                    {
                                                        MaxConnections = 0,
                                                        ConnectionType = 4,
                                                    },
                                                };

                                                LastMinuteDeals root = new LastMinuteDeals();

                                                PoolRequest ff = new PoolRequest();
                                                ff.Routes = new List<Models.Route>();
                                                ff.Routes.Add(route1);
                                                ff.Passengers = passengers;


                                                root.PoolRequest = ff;
                                                root.CurrencyCode = "EUR";

                                               var NewPool = JsonConvert.SerializeObject(root);
                                   
                                                lastMinuteDeal.PoolSearchRequest = NewPool;
                                                lastMinuteDeal.Title = "Izmir";
                                                lastMinuteDeal.Price = route[0].TotalPrice.Total;
                                                lastMinuteDeal.Airline = route[0].MarketingCarrier.Name;
                                                lastMinuteDeal.DepartureTime = FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Minute);
                                                lastMinuteDeal.ArrivalTime = FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Minute);
                                                lastMinuteDeal.ToLocation = dest;
                                                lastMinuteDeal.FromLocation = dept;
                                                lastMinuteDeal.Id = cout;
                                                lastMinuteDealsIzmir.Add(lastMinuteDeal);
                                                coutIzmir++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var OrderedListForIzmir = lastMinuteDealsIzmir.OrderBy(x => x.Price);
                int counterIzmir = 0;
                foreach (var pickItem in OrderedListForIzmir)
                {
                    if(counterIzmir <= 2)
                    {
                        LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();

                        lastMinuteDeal.PoolSearchRequest = pickItem.PoolSearchRequest;
                        lastMinuteDeal.Title = pickItem.Title;
                        lastMinuteDeal.Price = pickItem.Price;
                        lastMinuteDeal.Airline = pickItem.Airline;
                        lastMinuteDeal.DepartureTime = pickItem.DepartureTime;
                        lastMinuteDeal.ArrivalTime = pickItem.ArrivalTime;
                        lastMinuteDeal.ToLocation = pickItem.ToLocation;
                        lastMinuteDeal.FromLocation = pickItem.FromLocation;
                        lastMinuteDeal.UpdatedTime = DateTime.Now;
                        lastMinuteDeal.CreationTime = DateTime.Now;
                        lastMinuteDeal.IsActive = true;
                        lastMinuteDeal.ColumnNumber = 2;

                        _repository.LastMinuteDeals.CreateLastMinuteDeal(lastMinuteDeal);

                    }
                    else
                    {
                        break;
                    }
                    counterIzmir++;
                }

                #endregion

                #region KAYSERI
                List<LastMinuteDeal> lastMinuteDealsKayseri = new List<LastMinuteDeal>();
                var coutKayseri = 0;

                for (var i = 0; i <= noOfDayInLoop; i++)
                {
                    var departureDate = getNextYear + "-" + formattedNextMonth + "-" + (Convert.ToInt32(getNextDay) + i);

                    foreach (var dept in departureLocationKayseri)
                    {
                        foreach (var dest in destinationLocationKayseri)
                        {
                            if (dept != dest)
                            {
                                var airpoolSearchResponse = ProcessFlightSearch(dept, dest, departureDate);
                                if (airpoolSearchResponse != null)
                                {
                                    foreach (var items in airpoolSearchResponse?.Reader?.FareGroups)
                                    {
                                        foreach (var route in items.Routes)
                                        {
                                            if (destinationLocationKayseri.Contains(route[0].DestinationAirport.Code) && departureLocationKayseri.Contains(route[0].OriginAirport.Code))
                                            {

                                                LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();

                                                Passengers passengers = new Passengers();

                                                passengers.Adult = 1;
                                                passengers.Child = 0;
                                                passengers.Infant = 0;


                                                Models.Route route1 = new Models.Route()
                                                {
                                                    Origin = new Origin()
                                                    {
                                                        Code = dept,
                                                        IsCity = false,
                                                    },
                                                    Destination = new Destination()
                                                    {
                                                        Code = dest,
                                                        IsCity = false,
                                                    },
                                                    Departure = new Departure()
                                                    {
                                                        Date = departureDate,
                                                        DaysBefore = 1,
                                                        DaysAfter = 0,
                                                    },
                                                    FlightType = new FlightType()
                                                    {
                                                        MaxConnections = 0,
                                                        ConnectionType = 4,
                                                    },
                                                };

                                                LastMinuteDeals root = new LastMinuteDeals();

                                                PoolRequest ff = new PoolRequest();
                                                ff.Routes = new List<Models.Route>();
                                                ff.Routes.Add(route1);
                                                ff.Passengers = passengers;


                                                root.PoolRequest = ff;
                                                root.CurrencyCode = "EUR";

                                                var NewPool = JsonConvert.SerializeObject(root);

                                                lastMinuteDeal.PoolSearchRequest = NewPool;
                                                lastMinuteDeal.Title = "Kayseri";
                                                lastMinuteDeal.Price = route[0].TotalPrice.Total;
                                                lastMinuteDeal.Airline = route[0].MarketingCarrier.Name;
                                                lastMinuteDeal.DepartureTime = FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].DepartureTime.Minute);
                                                lastMinuteDeal.ArrivalTime = FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Hour) + ":" + FormatMintute(route[0].Segments[0].Legs[0].ArrivalTime.Minute);
                                                lastMinuteDeal.ToLocation = dest;
                                                lastMinuteDeal.FromLocation = dept;
                                                lastMinuteDeal.Id = cout;
                                                lastMinuteDealsKayseri.Add(lastMinuteDeal);
                                                coutKayseri++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var OrderedListForKayseri = lastMinuteDealsKayseri.OrderBy(x => x.Price);
                int counterKayseri = 0;
                foreach (var pickItem in OrderedListForKayseri)
                {
                    if (counterKayseri <= 2)
                    {
                        LastMinuteDeal lastMinuteDeal = new LastMinuteDeal();
                        lastMinuteDeal.PoolSearchRequest = pickItem.PoolSearchRequest;
                        lastMinuteDeal.Title = pickItem.Title;
                        lastMinuteDeal.Price = pickItem.Price;
                        lastMinuteDeal.Airline = pickItem.Airline;
                        lastMinuteDeal.DepartureTime = pickItem.DepartureTime;
                        lastMinuteDeal.ArrivalTime = pickItem.ArrivalTime;
                        lastMinuteDeal.ToLocation = pickItem.ToLocation;
                        lastMinuteDeal.FromLocation = pickItem.FromLocation;
                        lastMinuteDeal.UpdatedTime = DateTime.Now;
                        lastMinuteDeal.CreationTime = DateTime.Now;
                        lastMinuteDeal.IsActive = true;
                        lastMinuteDeal.ColumnNumber = 3;

                        _repository.LastMinuteDeals.UpdateLastMinuteDeal(lastMinuteDeal);

                    }
                    else
                    {
                        break;
                    }
                    counterKayseri++;
                }
                #endregion

            }
        }

        public AirpoolSearchResponseViewModel ProcessFlightSearch(string departureLocation, string destinationLocation, string departureDate)
        {
            try
            {
                var requestForLastMin = "{\'PoolRequest\':{\'Routes\':[{\'Origin\':{\'Code\':'" + departureLocation + "',\'IsCity\':false},\'Destination\':{\'Code\':'" + destinationLocation + "',\'IsCity\':false},\'Departure\':{\'Date\':'" + departureDate + "',\'DaysBefore\':1,\'DaysAfter\':0},\'FlightType\':{\'MaxConnections\':0,\'ConnectionType\':4}}],\'Passengers\':{\'Adult\':1,\'Child\':0,\'Infant\':0},\'Preference\':{\'RequiredCarrierCodes\':null,\'CabinType\':null}},\'CurrencyCode\':\'EUR\'}";
                PoolRequestBaseViewModel poolRequestBaseViewModel = JsonConvert.DeserializeObject<PoolRequestBaseViewModel>(requestForLastMin);
                string response = new StringFormatterHelper().ConvertIdAndRouteIdToString(new ApiRequestHelper().GetData("/api/airpool/search", poolRequestBaseViewModel).Result);
                var airpoolSearchResponse = JsonConvert.DeserializeObject<AirpoolSearchResponseViewModel>(response);
                return airpoolSearchResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string FormatMintute(int getMin)
        {
            var convertedMinute = getMin.ToString();
            if (!string.IsNullOrEmpty(convertedMinute))
            {
                if(convertedMinute.Length == 1)
                {
                    convertedMinute = "0" + convertedMinute;
                }
            }
            return convertedMinute;
        }
       
    }
}
