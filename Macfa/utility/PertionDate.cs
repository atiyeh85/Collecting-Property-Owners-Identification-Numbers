using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Macfa.utility
{
    public class PertionDate
    {
        public static string Today()
        {
            DateTime dt = DateTime.Now;

            System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();

            return p.GetYear(dt).ToString() + "/" + p.GetMonth(dt).ToString("0#") + "/" + p.GetDayOfMonth(dt).ToString("0#");
        }
        public static string PtoE(string Number)
        {
            string jj = Number;
            var Converted = Number.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
            return Converted;
        }
        public static string ConvertNumbersToEnglish(string str)
        {
            return str.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");
        }
        public static string EditString(string Rasteh,string nid)
        {
           
            var nidS = nid;
            var Y= nidS.Substring(0, 2);
            var R = nidS.Substring(2, 2);
            var c = nidS.Substring(4,5);
           
          var  CharsString = Y + "" + Rasteh + "" + c;
            return (CharsString);
        }
        public static string EditCoderahgiri(string personalcode,string beforecoderahgiri)

        {
            var befcod = beforecoderahgiri;
            var Years = befcod.Substring(0, 2);
            var str = Years + "" + personalcode;
            return str;

        }
        public static int CurrentMonth()
        {
            DateTime dt = DateTime.Now;

            System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();

            return p.GetMonth(dt);
        }
        public  static string GetAge(string date)
        {
            try
            {
                System.Globalization.PersianCalendar p = new System.Globalization.PersianCalendar();
                DateTime dt = p.ToDateTime(int.Parse(date.Substring(0, 4)),
                    int.Parse(date.Substring(5, 2)),
                    int.Parse(date.Substring(8, 2)), 0, 0, 0, 0);

                int age = (int)DateTime.Now.Subtract(dt).TotalDays / 365;
                string Age = Convert.ToString(age);
                return Age;
            }
            catch (Exception ex)
            {
            }
            return "1";

        }
    }
}