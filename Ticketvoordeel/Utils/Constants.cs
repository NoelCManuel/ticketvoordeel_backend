using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ticketvoordeel.Utils
{
    public static class Constants
    {
        public static string TurSysApiURL = "http://api.tursys.com/";

        public static string UserName = "ticketvoordeelnewapiuser";

        public static string Password = "ticketvoordeelnewapiuser2020";

        //public static string UserName = "ado.trv";

        //public static string Password = "ado123travel45";

        public static string ClientId = "TIC_bbwODY1sgDA7eo1D";

        public static string ClientSecret = "tic_faOCcXqUvyhDddt1";

        public static string TurSysTokenGenerationURL = "oauth2/token";

        public static string S3Url = "https://ticketvoordeel.s3.eu-central-1.amazonaws.com";

        //public static string ParkingURL = "https://qp-api.stag.typeqast.technology/";
        public static string ParkingURL = "https://qp-api.typeqast.technology/";        

        //public static string ParkingToken = "f0a2a9ac-af2d-4de9-bad4-46c4f0a5b2b1";
        public static string ParkingToken = "4f16141e-dd03-468e-b892-cd0fcf3b4c4f";        

        public static string SmsApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImp0aSI6ImY3NGQ5MDQyYmI4MmU5ZDNjZDdjNjAyZDcxN2VkYTIxZThjY2QzN2YxMDcyYmYwNjMxZWNhMmY3OWU0NGY1NGI3YWY5YjFjNGFkYTUxOTI1In0.eyJhdWQiOiIzIiwianRpIjoiZjc0ZDkwNDJiYjgyZTlkM2NkN2M2MDJkNzE3ZWRhMjFlOGNjZDM3ZjEwNzJiZjA2MzFlY2EyZjc5ZTQ0ZjU0YjdhZjliMWM0YWRhNTE5MjUiLCJpYXQiOjE2MTYwNjYyMjIsIm5iZiI6MTYxNjA2NjIyMiwiZXhwIjo0NzcxNzM5ODIyLCJzdWIiOiI1NjkzNSIsInNjb3BlcyI6W119.Jywg1BVyegeFadIzZAuwLgmwlzJ_w4VcOvoTyKm-eatoZhCfBNvWcdlglL6i9H2-7oZSvnd6DJeYn874qFB7L4BvDTaXzcRyryAA1e6IsbfZDDRYtrGIbCFBNAcZ3IzGqu6ii83iq-qhFB9qQjVVkvJjxD7gDTnYnRKd6XGw97P8-gvPg2BSwSvfxWFF1tty8mNdyJcyhvdz0GpEr4Qyh0v-L6IL5rLEUy0_j9UTEi308XO5CyiVrBgeYRIe-BrdcsLILSuFloDpBSHeXTpXPFVEF9FkDmJK-JkB42gzF--vdgEz1NkNArJ15UTeUy9RuMgSomrO3fh6qe29SrcaGq7N8UEf3fmqMpZdAw8Few3PTXsVZAgY8gPNenR1CSRCCvcFCjwf4DeVDlUjulSh6pXIqxObioPoS8fz7YkPO-2QSomEZeX2Y19EekBcV0as-WLDcfS83QRAIeSYS-MviSldwlM9830-GIKR67t3pZUqbPBrqCwXJWlwy0Q9aqZhbj3_Pb8epUz1OZ3OMGtxvZjoCJfP3QhVAaLhmHCOO9RIiwmhDEoIYKSDMJcPmFI6T0Lr8NJbkLDOXjjD3DsaVrxR45YNfd3R0g4r73osTeCzT-lT8s1yM3a7CILGc05nFbIG-eF_evz96WvOMBgStV53x7FPfcjU86RvTehHk40";

        public static string SmsApiURL = "https://rest.spryngsms.com/v1/";

        public enum LastMinuteDeals
        {
            Istanbul = 1,
            Izmir = 2,
            Kayseri = 3
        }

        public static string S3BaseURL = "https://ticketvoordeel.s3.eu-central-1.amazonaws.com/";
        public static string S3BaseURLLocal = "https://api.ticketvoordeel.nl/Uploads/";

        

        public static string MaxPackageCouponCode = "MAX20AKL";

        public static int MaxPackageAmount = 10;

        public static string MaxPackageName = "Korting MAX servicepakket";

        public static string SuvendusCouponCode = "SOVBON21TV";

        public static int SuvendusCouponAmount = 15;
    }
}
