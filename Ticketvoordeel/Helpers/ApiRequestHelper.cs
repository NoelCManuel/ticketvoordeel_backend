using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ticketvoordeel.Utils;

namespace Ticketvoordeel.Helpers
{
    public class ApiRequestHelper
    {
        public async Task<string> PostData(string url, string data)
        {
            try
            {
                LogToFile.writeLog(url);
                UriBuilder fullUri = new UriBuilder(url);
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                using (var client = new HttpClient(clientHandler))
                {
                    if (url.Contains(Utils.Constants.ParkingURL))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", Constants.ParkingToken);
                        LogToFile.ParkingVerifyDetails(data, "q");

                    }
                    LogToFile.writeLog(url + " : Request started");
                    HttpResponseMessage response = await client.PostAsync(new Uri(url), new StringContent(data, Encoding.UTF8, "application/json"));
                    LogToFile.writeLog(url + " : Request finished");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    string responseBody = string.Empty;
                    
                    if (response.IsSuccessStatusCode)
                    {
                        LogToFile.writeLog(url + " : API return success");
                        response.EnsureSuccessStatusCode();
                        responseBody = await response.Content.ReadAsStringAsync();
                        LogToFile.writeLog(url + " : API data read success");
                        if (url.Contains(Utils.Constants.ParkingURL))
                        {
                            LogToFile.ParkingVerifyDetails(responseBody, "b");
                        }
                    }
                    else
                    {
                        LogToFile.writeLog(url + " : API return error");
                        if (url.Contains(Utils.Constants.ParkingURL))
                        {
                            LogToFile.ParkingVerifyDetails("error", "b");
                        }
                    }

                    return responseBody;
                }                          
            }
            catch(Exception ex)
            {
                LogToFile.writeLog(url + " : Exception - " + ex.Message);
                if(url.Contains(Utils.Constants.ParkingURL))
                        {
                    LogToFile.ParkingVerifyDetails("error", "b");
                }
                return string.Empty;
            }            
        }

        public async Task<string> GetData(string url, object T)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                LogToFile.writeLog(url + JsonConvert.SerializeObject(T));
                using (var turSysClientList = new HttpClient())
                {
                    string token = AuthenticationHelper.GenerateToken();
                    string responseBody = String.Empty;
                    turSysClientList.BaseAddress = new Uri(Constants.TurSysApiURL);
                    turSysClientList.DefaultRequestHeaders.Accept.Clear();
                    turSysClientList.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    turSysClientList.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage turSysResponse = await turSysClientList.PostAsync(url, new StringContent(JsonConvert.SerializeObject(T), Encoding.UTF8, "application/json"));
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    if (url == "api/airpool/airports")
                    {
                        if (elapsedMs > 4000)
                        {
                            LogToFile.CheckAirportRequestResponseTime(elapsedMs, JsonConvert.SerializeObject(T), JsonConvert.SerializeObject(turSysResponse));
                        }
                    }
                    if (turSysResponse.IsSuccessStatusCode)
                    {
                        responseBody = await turSysResponse.Content.ReadAsStringAsync();
                        var responseBody1 = await turSysResponse.Content.ReadAsStreamAsync();
                        if (url.Contains("create") || url.Contains("prebook"))
                        {
                            LogToFile.writeErrorAPILog("URL");
                            LogToFile.writeErrorAPILog("Request: " + JsonConvert.SerializeObject(T));
                            LogToFile.writeErrorAPILog("Response: " + responseBody);
                        }                       
                        return responseBody;
                    }
                    else
                    {
                        LogToFile.writeLog(url + " : API return error");
                        return string.Empty;
                    }
                }
            }
            catch(Exception ex)
            {
                LogToFile.writeLog(url + " : Exception - " + ex.Message);
                return string.Empty;
            }                    
        }

        public async Task<string> Get(string url)
        {
            using (var turSysClientList = new HttpClient())
            {
                string token = AuthenticationHelper.GenerateToken();
                string responseBody = String.Empty;

                turSysClientList.BaseAddress = new Uri(Constants.TurSysApiURL);
                turSysClientList.DefaultRequestHeaders.Accept.Clear();
                turSysClientList.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                turSysClientList.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage turSysResponse = await turSysClientList.GetAsync(url);

                if (turSysResponse.IsSuccessStatusCode)
                {
                    responseBody = await turSysResponse.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
