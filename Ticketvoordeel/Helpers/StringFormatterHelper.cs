using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ticketvoordeel.Helpers
{
    public class StringFormatterHelper
    {
        public string ObjectToString(object T)
        {
            return new StringContent(JsonConvert.SerializeObject(T), Encoding.UTF8, "application/json").ToString();
        }

        public string ConvertIdAndRouteIdToString(string response)
        {
            string startString = ",\"Id\":";
            string endString = "\"IdStr\":";
            string startFairOptionString = ",\"RouteFareOptionId\":";
            List<int> idList = new List<int>();
            List<int> routeIdList = new List<int>();
            List<int> fairOptionIdList = new List<int>();
            idList = GetPositions(response, startString);
            routeIdList = GetPositions(response, endString);
            string updatedString = String.Empty;
            string updatedStringLatest = String.Empty;
            ulong ul = 0;

            if (idList.Count == routeIdList.Count && idList.Count > 0)
            {
                for (int i = 0; i < idList.Count; i++)
                {
                    string oldString = response.Substring(idList[i] + 6, (routeIdList[i] - (idList[i] + 7)));
                    try
                    {
                        ul = Convert.ToUInt64(response.Substring(idList[i] + 6, (routeIdList[i] - (idList[i] + 7))));
                    }
                    catch (Exception x)
                    {

                    }
                    string newString = "\"" + ul.ToString() + "\"";
                    if (updatedString.Length > 0)
                    {
                        updatedString = updatedString.Replace(oldString, newString);
                    }
                    else
                    {
                        updatedString = response.Replace(oldString, newString);
                    }
                }
            }

            routeIdList = GetPositions(updatedString, startFairOptionString);
            for (int i = 0; i < routeIdList.Count; i++)
            {
                string substring = updatedString.Substring(routeIdList[i] + 21, 22);
                int commaPosition = substring.IndexOf(",");
                try
                {
                    ul = Convert.ToUInt64(substring.Substring(0, commaPosition));
                }
                catch (Exception ex)
                {

                }
                string newString = "\"" + ul.ToString() + "\"";
                if (updatedStringLatest.Length > 0)
                {
                    updatedStringLatest = updatedStringLatest.Replace(ul.ToString(), newString);
                }
                else
                {
                    updatedStringLatest = updatedString.Replace(ul.ToString(), newString);
                }
            }

            updatedStringLatest = updatedStringLatest.Replace("\"\"\"\"\"", "\"");
            updatedStringLatest = updatedStringLatest.Replace("\"\"\"\"", "\"");
            updatedStringLatest = updatedStringLatest.Replace("\"\"\"", "\"");
            updatedStringLatest = updatedStringLatest.Replace("\"\"", "\"");

            return updatedStringLatest;
        }

        public List<int> GetPositions(string source, string searchString)
        {
            List<int> ret = new List<int>();
            int len = searchString.Length;
            int start = -len;
            while (true)
            {
                start = source.IndexOf(searchString, start + len);
                if (start == -1)
                {
                    break;
                }
                else
                {
                    ret.Add(start);
                }
            }
            return ret;
        }

        public string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public string RemoveFlightNumber(string flightCode)
        {
            try
            {
                var flightNumber = string.Empty;
                if(!string.IsNullOrEmpty(flightCode) && flightCode.Contains("TK"))
                {
                    var splitFligthDetail = flightCode.Split("TK");
                    if(splitFligthDetail != null && splitFligthDetail.Count() == 2)
                    {
                        flightNumber = splitFligthDetail[1];
                    }
                }
                else
                {
                    return "NOTK";
                }
                return flightNumber;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
