using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Helpers
{
    public static class LogToFile
    {
        public static void writeLog(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("logfile.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }
        public static void PaymentUrl(string content, string step)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("PaymentUrl.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "   step" + step + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }
        public static void SpecficPaymentUrl(string content, string step)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("SpecficPaymentUrl.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "   step" + step + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }
        public static void RecordSunExpressRemainderMail(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("RecordSunExpressRemainderMail.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }


        public static void writeBookingLog(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("bookinglogfile.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }

        public static void writePaymentLog(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("paymentlogfile.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }

        public static void writeErrorAPILog(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("errorAPIcalllogfile.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                }
            }
            catch
            {

            }
        }

        public static void AdminMailForCredit(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("AdminMailForCredit.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }
        public static void AdminMailForCredit1(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("AdminMailForCredit1.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }

        public static void CheckFailureReport(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("CheckFailureReport.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }

        public static void ChangeFolderPath(string content,int id)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("ChangeFolderPath.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(id);
                    writer.WriteLine(content);
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }
        public static void ParkingLogRes(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("ChangeFolderPath.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                    writer.WriteLine("");
                }
            }
            catch
            {

            }
        }

        public static void CheckAirportRequestResponseTime(long endTime, string request, string response)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("CheckAirportRequestResponseTime.txt"))
                {
                    writer.WriteLine("");
                    writer.WriteLine("--------------------------Start-----------------------");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(endTime);
                    writer.WriteLine("");

                    writer.WriteLine("");
                    writer.WriteLine(request);
                    writer.WriteLine("");

                    writer.WriteLine("");
                    writer.WriteLine(response);
                    writer.WriteLine("");

                    writer.WriteLine("");
                    writer.WriteLine("--------------------------End-----------------------");

                }
            }
            catch
            {

            }
        }

        public static void ParkingVerifyDetails(string content, string type)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("ParkingVerifyDetails.txt"))
                {
                    if (type == "q")
                    {
                        writer.WriteLine("Quoting");
                        writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                        writer.WriteLine(content);
                        writer.WriteLine("");
                    }
                    if (type == "b")
                    {
                        writer.WriteLine("Booking");
                        writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                        writer.WriteLine(content);
                        writer.WriteLine("");
                    }
                }
            }
            catch
            {

            }
        }

        public static void RecordIncompleteBooking(string content)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText("RecordIncompleteBooking.txt"))
                {

                    writer.WriteLine("Booking");
                    writer.WriteLine("--------------------------" + DateTime.Now + "-----------------------");
                    writer.WriteLine(content);
                    writer.WriteLine("");

                }
            }
            catch
            {

            }
        }

        

    }
}
