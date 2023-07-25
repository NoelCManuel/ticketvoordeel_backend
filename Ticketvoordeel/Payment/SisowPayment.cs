using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ticketvoordeel.Models;

namespace Ticketvoordeel.Payment
{
    public class SisowPaymentGateway
    {
        private static string[] issuerid;
        private static string[] issuername;
        private static DateTime lastcheck;

        private string response;

        // Merchant data
        public string merchantId;
        public string merchantKey;

        // Transaction data
        public string payment;          // empty=iDEAL; sofort=SofortBanking; mistercash=MisterCash; overboeking=OverBoeking; ecare=Sisow ecare; ...
        public string issuerId;         // mandatory for iDEAL; Sisow iDEAL bank code
        public string purchaseId;       // mandatory; max 16 alphanumeric
        public string entranceCode;     // max 40 strict alphanumeric (letters and numbers only)
        public string description;      // mandatory; max 32 alphanumeric
        public double amount;           // mandatory; min 0.45
        public string notifyUrl;
        public string returnUrl;        // mandatory
        public string cancelUrl;
        public string callbackUrl;
        public bool testMode;
        public string shopId;

        // Invoice data
        public string invoiceNo;
        public long documentId;
        public string documentUrl;

        // Status data
        public string status;
        public DateTime timeStamp;
        public string consumerAccount;
        public string consumerName;
        public string consumerCity;

        // Result/check data
        public string trxId;
        public string issuerUrl;

        // Error data
        public string errorCode;
        public string errorMessage;

        // Status
        public const string statusSuccess = "Success";
        public const string statusCancelled = "Cancelled";
        public const string statusExpired = "Expired";
        public const string statusFailure = "Failure";
        public const string statusOpen = "Open";

        public SisowPaymentGateway()
        {   //TV
            this.merchantId = "2537501503";
            this.merchantKey = "4e1f177796be91cbc3a0fb4635c7cc7ab0235a7f";
            //Ado
            //this.merchantId = "2537490076";
            //this.merchantKey = "4cdd16d2b53edced46976fd124fb38b54e500f9a";
            testMode = false;
            //credit card test
            //this.payment = "creditcard";
        }

        private async Task<bool> send(string method)
        {
            return await sendAsync(method, null, null);
        }

        private async Task<bool> send(string method, string[] keyvalue)
        {
            return await sendAsync(method, keyvalue, null);
        }

        private async Task<bool> sendAsync(string method, string[] keyvalue, string[] extra)
        {
            string parms = "";
            string url = "https://www.sisow.nl/Sisow/iDeal/RestHandler.ashx/" + method;
            try
            {
                if (keyvalue != null && keyvalue.Length > 0)
                {
                    for (int i = 0; i + 1 < keyvalue.Length; i += 2)
                    {
                        if (string.IsNullOrEmpty(keyvalue[i + 1]))
                            continue;
                        if (!string.IsNullOrEmpty(parms))
                            parms += "&";
                        parms += keyvalue[i] + "=" + System.Net.WebUtility.HtmlEncode(keyvalue[i + 1]);
                    }
                }
                if (extra != null && extra.Length > 0)
                {
                    for (int i = 0; i + 1 < extra.Length; i += 2)
                    {
                        if (string.IsNullOrEmpty(extra[i + 1]))
                            continue;
                        if (!string.IsNullOrEmpty(parms))
                            parms += "&";
                        parms += extra[i] + "=" + System.Net.WebUtility.HtmlEncode(extra[i + 1]);
                    }
                }

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    if (method == "TransactionRequest" || method == "StatusRequest")
                    {
                        HttpResponseMessage wcfResponse = await httpClient.GetAsync(url + "?" + parms);
                        response = await wcfResponse.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        HttpResponseMessage wcfResponse = await httpClient.PostAsync(url, new StringContent(parms, Encoding.UTF8, "application/json"));
                        response = await wcfResponse.Content.ReadAsStringAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                response = "";
                errorMessage = ex.Message;
                return false;
            }
        }

        private string parse(string search)
        {
            return parse(response, search);
        }

        private void error()
        {
            errorCode = parse("errorcode");
            errorMessage = System.Net.WebUtility.HtmlDecode(parse("errormessage"));
        }

        private string parse(string xml, string search)
        {
            int start, end;

            if ((start = xml.IndexOf("<" + search + ">")) < 0)
                return null;
            start += search.Length + 2;
            if ((end = xml.IndexOf("</" + search + ">", start)) < 0)
                return null;
            return xml.Substring(start, end - start);
        }

        private async Task<int> getDirectory()
        {
            if (issuerid != null && lastcheck.AddDays(1).CompareTo(DateTime.Now) >= 0)
                return 0;
            if (!await send("DirectoryRequest"))
                return -1;
            string search = parse("directory");
            if (string.IsNullOrEmpty(search))
            {
                error();
                return -2;
            }
            string[] iss = search.Replace("<issuer>", "").Split(new string[] { "</issuer>" }, StringSplitOptions.RemoveEmptyEntries);
            issuerid = new string[iss.Length];
            issuername = new string[iss.Length];
            for (int i = 0; i < iss.Length; i++)
            {
                issuerid[i] = parse(iss[i], "issuerid");
                issuername[i] = parse(iss[i], "issuername");
            }
            lastcheck = DateTime.Now;
            return 0;
        }

        // DirectoryRequest
        public async Task<List<SisowIssuer>> DirectoryRequestAsync()
        {
            List<SisowIssuer> SisowIssuerList = new List<SisowIssuer>();
            SisowIssuer sisowIssuer;

            int ex;
            ex = await getDirectory();
            if (ex < 0)
                return new List<SisowIssuer>();
            for (int i = 0; i < issuerid.Length; i++)
            {
                sisowIssuer = new SisowIssuer();
                sisowIssuer.IssueId = issuerid[i];
                sisowIssuer.IssuerName = issuername[i];

                SisowIssuerList.Add(sisowIssuer);
            }

            return SisowIssuerList;
        }

        // DirectoryRequest
        public async Task<Tuple<int, bool, string[]>> DirectoryRequest(bool test)
        {
            int ex;
            string[] issuers;
            issuers = null;
            ex = await getDirectory();
            if (ex < 0)
                return new Tuple<int, bool, string[]>(ex, test, issuers);

            issuers = new string[issuerid.Length * 2];
            for (int i = 0; i < issuerid.Length; i++)
            {
                issuers[i * 2] = issuerid[i];
                issuers[i * 2 + 1] = issuername[i];
            }
            return new Tuple<int, bool, string[]>(0, test, issuers);
        }

        // TransactionRequest
        public async Task<int> TransactionRequest()
        {
            trxId = issuerUrl = "";
            string[] extra = null;
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(purchaseId))
            {
                errorMessage = "No purchaseid";
                return -3;
            }
            if (amount < 0.45)
            {
                errorMessage = "amount < 0.45";
                return -4;
            }
            if (string.IsNullOrEmpty(description))
            {
                errorMessage = "No description";
                return -5;
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                errorMessage = "No returnurl";
                return -6;
            }
            if (string.IsNullOrEmpty(issuerId) && string.IsNullOrEmpty(payment))
            {
                errorMessage = "No iDEAL issuerid or no payment";
                return -7;
            }
            if (string.IsNullOrEmpty(entranceCode))
                entranceCode = purchaseId;
            string sha1 = GetSHA1(purchaseId + entranceCode + Math.Round(amount * 100).ToString() + shopId + merchantId + merchantKey);
            string[] pars = { "merchantid", merchantId, "payment", payment, "issuerid", issuerId, "purchaseid", purchaseId,
            "amount", Math.Round(amount * 100).ToString(), "description", description, "entrancecode", entranceCode, "returnurl", returnUrl,
            "cancelurl", cancelUrl, "callbackurl", callbackUrl, "notifyurl", notifyUrl, "testmode", (testMode ? "true" : "false"), "sha1", sha1 };
            if (!await sendAsync("TransactionRequest", pars, extra))
                return -8;
            trxId = parse("trxid");
            if (string.IsNullOrEmpty(trxId))
            {
                error();
                return -2;
            }
            issuerUrl = System.Net.WebUtility.UrlDecode(parse("issuerurl"));
            invoiceNo = parse("invoiceno");
            long.TryParse(parse("documentid"), out documentId);
            documentUrl = System.Net.WebUtility.HtmlDecode(parse("documenturl"));
            return 0;
        }

        // compute SHA1
        private static string GetSHA1(string key)
        {
            //var sha1 = System.Security.Cryptography.SHA1.Create();
            //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            //byte[] bytes = sha1.ComputeHash(enc.GetBytes(key));
            ////string sha1 = System.BitConverter.ToString(sha1).Replace("-", "");
            ////string sha1 = "";
            //for (int j = 0; j < bytes.Length; j++)
            //    sha1 += bytes[j].ToString("x2");
            //return sha1;

            byte[] bytes = Encoding.UTF8.GetBytes(key);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        // TransactionRequest
        public async Task<int> TransactionRequest(params string[] keyvalue)
        {
            trxId = issuerUrl = "";
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(purchaseId))
            {
                errorMessage = "No purchaseid";
                return -3;
            }
            if (amount < 0.45)
            {
                errorMessage = "amount < 0.45";
                return -4;
            }
            if (string.IsNullOrEmpty(description))
            {
                errorMessage = "No description";
                return -5;
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                errorMessage = "No returnurl";
                return -6;
            }
            if (string.IsNullOrEmpty(issuerId) && string.IsNullOrEmpty(payment))
            {
                errorMessage = "No iDEAL issuerid or no payment";
                return -7;
            }
            if (string.IsNullOrEmpty(entranceCode))
                entranceCode = purchaseId;
            string sha1 = GetSHA1(purchaseId + entranceCode + (amount * 100).ToString() + shopId + merchantId + merchantKey);
            string[] pars = { "merchantid", merchantId, "payment", payment, "issuerid", issuerId, "purchaseid", purchaseId,
            "amount", Math.Round(amount * 100).ToString(), "description", description, "entrancecode", entranceCode, "returnurl", returnUrl,
            "cancelurl", cancelUrl, "callbackurl", callbackUrl, "notifyurl", notifyUrl, "testmode", (testMode ? "true" : "false"), "sha1", sha1 };
            if (!await sendAsync("TransactionRequest", pars, keyvalue))
                return -8;
            trxId = parse("trxid");
            if (string.IsNullOrEmpty(trxId))
            {
                error();
                return -2;
            }
            issuerUrl = System.Net.WebUtility.HtmlDecode(parse("issuerurl"));
            invoiceNo = parse("invoiceno");
            long.TryParse(parse("documentid"), out documentId);
            documentUrl = System.Net.WebUtility.HtmlDecode(parse("documenturl"));
            return 0;
        }

        private int GetStatus()
        {
            status = parse("status");
            if (string.IsNullOrEmpty(status))
            {
                error();
                return -5;
            }
            timeStamp = DateTime.Parse(parse("timestamp"));
            amount = long.Parse(parse("amount")) / 100.0;
            consumerAccount = parse("consumeraccount");
            consumerName = parse("consumername");
            consumerCity = parse("consumercity");
            purchaseId = parse("purchaseid");
            description = parse("description");
            entranceCode = parse("entrancecode");
            return 0;
        }

        // StatusRequest
        public async Task<int> StatusRequest()
        {
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(trxId))
            {
                errorMessage = "No trxid";
                return -3;
            }
            string sha1 = GetSHA1(trxId + merchantId + merchantKey);
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!await send("StatusRequest", pars))
                return -4;
            return GetStatus();
        }

        // StatusRequest
        public async Task<int> StatusRequest(string trxid)
        {
            if (string.IsNullOrEmpty(merchantId))
            {
                errorMessage = "No Merchant ID";
                return -1;
            }
            if (string.IsNullOrEmpty(merchantKey))
            {
                errorMessage = "No Merchant Key";
                return -2;
            }
            if (string.IsNullOrEmpty(trxid))
            {
                errorMessage = "No trxid";
                return -3;
            }
            trxId = trxid;
            string sha1 = GetSHA1(trxId + merchantId + merchantKey);
            string[] pars = { "merchantid", merchantId, "trxid", trxId, "sha1", sha1 };
            if (!await send("StatusRequest", pars))
                return -4;
            return GetStatus();
        }
    }
}
