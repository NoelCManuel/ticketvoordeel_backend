using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ticketvoordeel.Models.BookRequest;

namespace Ticketvoordeel.Helpers
{
    public class InsuranceHelper
    {
        public string CheckCancellationInsurancePremium(string startDate, decimal packageAmount)
        {
            try
            {
                packageAmount = Math.Round(packageAmount, 0);
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:v1='http://namespace.europeesche.nl/premieservice/v1'> <soapenv:Header/> <soapenv:Body> <v1:berekenPremie14ANNULRequest><PREMIEREQUEST><AL><AL_ENTITEI>AL</AL_ENTITEI><AL_FUNCTIE>13</AL_FUNCTIE><AL_RECCRT>483958</AL_RECCRT><AL_VIEWCOD>53045</AL_VIEWCOD></AL><AN><AN_ENTITEI>AN</AN_ENTITEI><AN_MYCODE>3019</AN_MYCODE><AN_VERZSOM>" + packageAmount + "</AN_VERZSOM></AN><PP><PP_ENTITEI>PP</PP_ENTITEI><PP_INGDAT>" + startDate + "</PP_INGDAT><PP_PRODUCT>14ANNUL</PP_PRODUCT></PP></PREMIEREQUEST></v1:berekenPremie14ANNULRequest></soapenv:Body></soapenv:Envelope>");
                HttpWebRequest webRequest = CreateWebRequest("https://eol.services.europeesche.nl/int/premieservice/premie", "http://tempuri.org/IPremieBerekenenService/berekenPremie14ANNUL");
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest, true).Wait();

                var asyncResult = webRequest.GetResponseAsync();
                asyncResult.Wait();

                HttpWebResponse response = (HttpWebResponse)asyncResult.Result;
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                string soapResult = readStream.ReadToEnd();

                int PricePos1 = soapResult.IndexOf("<PP_TTOT>") + "<PP_TTOT>".Length;
                int PricePos2 = soapResult.IndexOf("</PP_TTOT>");
                var price = soapResult.Substring(PricePos1, PricePos2 - PricePos1).Contains(',') ? soapResult.Substring(PricePos1, PricePos2 - PricePos1).Replace(',', '.') : soapResult.Substring(PricePos1, PricePos2 - PricePos1);

                return "{'PolicyNumber':'', 'Amount':" + price + "}";
            }
            catch (Exception e)
            {
                return "{'PolicyNumber':'', 'Amount':0}";
            }
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers["SOAPAction"] = action;
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            return webRequest;
        }
        private static XmlDocument CreateSoapEnvelope(string envelope)
        {
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(envelope);
            return soapEnvelopeDocument;
        }
        private static async void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        private static async Task InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest, bool isTrue)
        {
            using (Stream stream = await webRequest.GetRequestStreamAsync())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
        public string BookCancellationInsurance(decimal packageAmount, string pnr, string packageStartDate, string packageEndDate, string passengerInfo)
        {
            try
            {
                XmlDocument soapEnvelopeXml = CreateSoapEnvelope("<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:v1='http://namespace.europeesche.nl/verzekeringservice/v1'> <soapenv:Header/> <soapenv:Body> <v1:maakContract14ANNULRequest><CONTRACTREQUEST><AL><AL_ENTITEI>AL</AL_ENTITEI><AL_FUNCTIE>01</AL_FUNCTIE><AL_RECCRT>697591</AL_RECCRT><AL_VIEWCOD>53045</AL_VIEWCOD></AL><AN><AN_ENTITEI>AN</AN_ENTITEI><AN_MYCODE>3019</AN_MYCODE><AN_VERZSOM>" + packageAmount + " </AN_VERZSOM></AN><CW><CW_ENTITEI>CW</CW_ENTITEI><CW_INFO>TP</CW_INFO></CW><PP><PP_ENTITEI>PP</PP_ENTITEI><PP_BETWIJZ>T</PP_BETWIJZ><PP_EXTERN>" + pnr + "</PP_EXTERN><PP_INCWIJZ>A</PP_INCWIJZ><PP_INGDAT>" + FormatInsuranceDate(DateTime.Now) + "</PP_INGDAT><PP_LANDBEL>NL</PP_LANDBEL><PP_PRODUCT>14ANNUL</PP_PRODUCT></PP><RI><RI_ENTITEI>RI</RI_ENTITEI><RI_ALGVRWD>J</RI_ALGVRWD><RI_EGD>N</RI_EGD><RI_EGDO/><RI_FRAUD>N</RI_FRAUD><RI_OGB>N</RI_OGB><RI_POLJUS>N</RI_POLJUS></RI><RS><RS_ENTITEI>RS</RS_ENTITEI><RS_ENDRDT>" + packageEndDate + "</RS_ENDRDT><RS_INGRDT>" + packageStartDate + "</RS_INGRDT><RS_REIDAT>" + FormatInsuranceDate(DateTime.Now) + "</RS_REIDAT></RS>" + passengerInfo + "</CONTRACTREQUEST></v1:maakContract14ANNULRequest></soapenv:Body></soapenv:Envelope>");
                HttpWebRequest webRequest = CreateWebRequest("https://eol.services.europeesche.nl/int/verzekeringservice/verzekeringafsluiten", "http://tempuri.org/IVerzekeringAfsluitenService/maakContract14ANNUL");
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest, true).Wait();

                var asyncResult = webRequest.GetResponseAsync();
                asyncResult.Wait();

                HttpWebResponse response = (HttpWebResponse)asyncResult.Result;
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                string soapResult = readStream.ReadToEnd();

                int PolicyNumberPos1 = soapResult.IndexOf("<PP_NUMMER>") + "<PP_NUMMER>".Length;
                int PolicyNumberPos2 = soapResult.IndexOf("</PP_NUMMER>");
                var policyNumber = soapResult.Substring(PolicyNumberPos1, PolicyNumberPos2 - PolicyNumberPos1);
                return policyNumber.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }
        public static string FormatInsuranceDate(DateTime parsedDateTime)
        {
            string formattedDate = string.Empty;
            try
            {
                formattedDate = parsedDateTime.ToString("yyyy") + parsedDateTime.ToString("MM") + parsedDateTime.ToString("dd");
            }
            catch (Exception ex)
            {
                return formattedDate;
            }

            return formattedDate;
        }
        public string GeneratePassengerInfoForCancellationBooking(MainBooker mainbooker, List<Passenger> passengers)
        {
            string dateOfBirth = String.Empty;
            string data = String.Empty;
            int i = 0;

            string addressLine = mainbooker.Street;
            string houseNumber = mainbooker.HouseNumber;
            string postCode = mainbooker.PostCode;

            foreach (var items in passengers)
            {
                i++;
                if (i == 1)
                {
                    data += "<VP><VP_ENTITEI>VP</VP_ENTITEI><VP_ANAAM>" 
                        + mainbooker.FirstName + 
                        "</VP_ANAAM><VP_EMAIL>" + mainbooker.Email + 
                        "</VP_EMAIL><VP_GEBDAT>" + items.Birthdate + 
                        "</VP_GEBDAT><VP_GESLACH>M</VP_GESLACH><VP_HUISNR>" + houseNumber + 
                        "</VP_HUISNR><VP_LAND>NL</VP_LAND><VP_NIEUWSB>N</VP_NIEUWSB><VP_PCODE>" + postCode + 
                        "</VP_PCODE><VP_PLAATS>" + mainbooker.Place + 
                        "</VP_PLAATS><VP_STRAAT>" + addressLine + " </VP_STRAAT><VP_VOORL>" + 
                        mainbooker.LastName + "</VP_VOORL></VP>";
                }
                data += "<VZ><VZ_ENTITEI>VZ</VZ_ENTITEI><VZ_ANAAM>" + items.FirstName + "</VZ_ANAAM><VZ_GEBDAT>" + items.Birthdate + "</VZ_GEBDAT><VZ_VOLGNUM>" + i + "</VZ_VOLGNUM><VZ_VOORL>" + items.LastName + "</VZ_VOORL></VZ>";
            }
            return data;
        }
    }
}
