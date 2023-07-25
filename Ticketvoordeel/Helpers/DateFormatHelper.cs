using Amazon.S3.Model.Internal.MarshallTransformations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Helpers
{
    public class DateFormatHelper
    {
        public string ChangeDate(string startDate, int numberOfDays)
        {
            string formattedDate = startDate.Substring(0, 4) + "-" + startDate.Substring(4, 2) + "-" + startDate.Substring(6, 2);
            DateTime generatedStartDate = DateTime.ParseExact(formattedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            generatedStartDate = generatedStartDate.AddDays(numberOfDays);

            return generatedStartDate.Date.Year.ToString() + generatedStartDate.ToString("MM") + generatedStartDate.ToString("dd");
        }

        public string FormatDateForSMS(string date)
        { 
            string Day = date.Substring(6, 2);
            string Month = date.Substring(4, 2);
            return Day + generateMonth(Month);
        }

        public string FormatDateForMail(string date)
        {
            string Day = date.Substring(6, 2);
            string Month = date.Substring(4, 2);
            string Year = date.Substring(0, 4);
            return Day + "-" + generateMonth(Month) + "-" + Year;
        }

        private string generateMonth(string Month)
        {
            switch (Month)
            {
                case "01":
                    return "JAN";
                case "02":
                    return "FEB";
                case "03":
                    return "MAR";
                case "04":
                    return "APR";
                case "05":
                    return "MAY";
                case "06":
                    return "JUN";
                case "07":
                    return "JUL";
                case "08":
                    return "AUG";
                case "09":
                    return "SEP";
                case "10":
                    return "OCT";
                case "11":
                    return "NOV";
                case "12":
                    return "DEC";
                default:
                    return "";
            }
        }
    }
}
