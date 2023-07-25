using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Ticketvoordeel.Helpers;
using Ticketvoordeel.Models;
using Ticketvoordeel.Models.AirpoolSearchResponse;

namespace Ticketvoordeel.Controllers
{
    public class DataSyncController : Controller
    {
        private IRepositoryWrapper _repository;
        List<Deals> DealsList = new List<Deals>();

        public DataSyncController(IRepositoryWrapper repository)
        {
            _repository = repository;

            DealsList = new List<Deals>();

            DealsList.Add(new Deals { 
                FromLocation = "AMS",
                ToLocation = "SAW"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "AMS",
                ToLocation = "IST"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "AMS",
                ToLocation = "DUS"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "AMS",
                ToLocation = "ADB"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "AMS",
                ToLocation = "ADB"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "AMS",
                ToLocation = "ASR"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "DUS",
                ToLocation = "ASR"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "ASR",
                ToLocation = "AMS"
            });
            DealsList.Add(new Deals
            {
                FromLocation = "ADB",
                ToLocation = "AMS"
            });
        }

        [AllowAnonymous]
        [Route("synclastminutedeals")]
        public JsonResult SyncLastMinuteDeals()
        {
            try
            {
                var lastMinuteDeals = _repository.LastMinuteDeals.GetAllLastMinuteDeals();
                LastMinuteDeal lastMinuteDeal;
                AirpoolSearchResponseViewModel airpoolSearchResponseViewModel;                

                foreach (var items in lastMinuteDeals)
                {
                    if(items.Id == 9)
                    {
                        int a = 0;
                    }
                    var response = new ApiRequestHelper().GetData("/api/airpool/search", JsonConvert.DeserializeObject<LastMinuteDeals>(items.PoolSearchRequest)).Result;
                    lastMinuteDeal = new LastMinuteDeal();
                    lastMinuteDeal = items;
                    bool setInitialPrice = false;

                    if (response != null)
                    {
                        airpoolSearchResponseViewModel = new AirpoolSearchResponseViewModel();
                        airpoolSearchResponseViewModel = JsonConvert.DeserializeObject<AirpoolSearchResponseViewModel>(response);
                        foreach(var fareGroups in airpoolSearchResponseViewModel.Reader.FareGroups)
                        {
                            for(int i = 0; i < fareGroups.Routes.Count; i++)
                            {
                                foreach (var routes in fareGroups.Routes[i])
                                {
                                    if(routes.TotalPrice.Total < lastMinuteDeal.Price)
                                    {
                                        if (setInitialPrice == false)
                                        {
                                            lastMinuteDeal.Price = routes.TotalPrice.Total;
                                            lastMinuteDeal.Airline = routes.MarketingCarrier.Name;
                                            lastMinuteDeal.DepartureTime = routes.Segments[0].Legs[0].DepartureTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].DepartureTime.Minute.ToString();
                                            lastMinuteDeal.ArrivalTime = routes.Segments[0].Legs[0].ArrivalTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].ArrivalTime.Minute.ToString();
                                            setInitialPrice = true;
                                        }
                                        else
                                        {
                                            if (routes.TotalPrice.Total < lastMinuteDeal.Price)
                                            {
                                                lastMinuteDeal.Price = routes.TotalPrice.Total;
                                                lastMinuteDeal.Airline = routes.MarketingCarrier.Name;
                                                lastMinuteDeal.DepartureTime = routes.Segments[0].Legs[0].DepartureTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].DepartureTime.Minute.ToString();
                                                lastMinuteDeal.ArrivalTime = routes.Segments[0].Legs[0].ArrivalTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].ArrivalTime.Minute.ToString();
                                            }
                                        }
                                    }                                    
                                }
                            }                           
                        }                        
                    }
                    _repository.LastMinuteDeals.UpdateLastMinuteDeal(lastMinuteDeal);
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("synclastminutedealsupdated")]
        public JsonResult SyncLastMinuteDealsUpdated()
        {
            try
            {
                List<LastMinuteDeal> LastMinuteDeals = (List<LastMinuteDeal>)_repository.LastMinuteDeals.GetAllLastMinuteDeals();
                LastMinuteDeal lastMinuteDeal;
                AirpoolSearchResponseViewModel airpoolSearchResponseViewModel;
                LastMinuteDeals searchRequest;
                int counter = 0;

                foreach (var deal in DealsList)
                {
                    lastMinuteDeal = new LastMinuteDeal();
                    searchRequest = new LastMinuteDeals();
                    lastMinuteDeal = LastMinuteDeals[counter];
                    searchRequest = JsonConvert.DeserializeObject<LastMinuteDeals>(lastMinuteDeal.PoolSearchRequest);

                    searchRequest.PoolRequest.Routes[0].Origin.Code = deal.FromLocation;
                    searchRequest.PoolRequest.Routes[0].Destination.Code = deal.ToLocation;
                    searchRequest.PoolRequest.Routes[0].Departure.Date = GenerateDate();

                    lastMinuteDeal.PoolSearchRequest = JsonConvert.SerializeObject(searchRequest);
                    lastMinuteDeal.Price = int.MaxValue;

                    var response = new ApiRequestHelper().GetData("/api/airpool/search", searchRequest).Result;
                    bool setInitialPrice = false;

                    if (response != null)
                    {
                        airpoolSearchResponseViewModel = new AirpoolSearchResponseViewModel();
                        airpoolSearchResponseViewModel = JsonConvert.DeserializeObject<AirpoolSearchResponseViewModel>(response);
                        foreach (var fareGroups in airpoolSearchResponseViewModel.Reader.FareGroups)
                        {
                            for (int i = 0; i < fareGroups.Routes.Count; i++)
                            {
                                foreach (var routes in fareGroups.Routes[i])
                                {
                                    if (routes.TotalPrice.Total < lastMinuteDeal.Price)
                                    {
                                        if (setInitialPrice == false)
                                        {
                                            lastMinuteDeal.FromLocation = deal.FromLocation;
                                            lastMinuteDeal.ToLocation = deal.ToLocation;
                                            lastMinuteDeal.Price = routes.TotalPrice.Total;
                                            lastMinuteDeal.Airline = routes.MarketingCarrier.Name;
                                            lastMinuteDeal.DepartureTime = routes.Segments[0].Legs[0].DepartureTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].DepartureTime.Minute.ToString();
                                            lastMinuteDeal.ArrivalTime = routes.Segments[0].Legs[0].ArrivalTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].ArrivalTime.Minute.ToString();
                                            setInitialPrice = true;
                                        }
                                        else
                                        {
                                            if (routes.TotalPrice.Total < lastMinuteDeal.Price)
                                            {
                                                lastMinuteDeal.FromLocation = deal.FromLocation;
                                                lastMinuteDeal.ToLocation = deal.ToLocation;
                                                lastMinuteDeal.Price = routes.TotalPrice.Total;
                                                lastMinuteDeal.Airline = routes.MarketingCarrier.Name;
                                                lastMinuteDeal.DepartureTime = routes.Segments[0].Legs[0].DepartureTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].DepartureTime.Minute.ToString();
                                                lastMinuteDeal.ArrivalTime = routes.Segments[0].Legs[0].ArrivalTime.Hour.ToString() + ":" + routes.Segments[0].Legs[0].ArrivalTime.Minute.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    _repository.LastMinuteDeals.UpdateLastMinuteDeal(lastMinuteDeal);
                    counter++;
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public class Deals
        { 
            public string FromLocation { get; set; }
            public string ToLocation { get; set; }
        }

        public string GenerateDate()
        {
            return DateTime.Now.AddDays(30).Year + "-"
                + DateTime.Now.AddDays(30).Month + "-"
                + DateTime.Now.AddDays(30).Day;
        }
    }
}