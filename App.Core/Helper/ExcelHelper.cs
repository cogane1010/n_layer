using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace App.Core.Helper
{
    public class ExcelHelper
    {
        public static void SetExcelNumberFormat(ExcelRange range)
        {
            range.Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
        }

        public static void SetExcelNumberFormat1(ExcelRange range)
        {
            range.Style.Numberformat.Format = "_(* #,##0.0_);_(* (#,##0.0);_(* \" - \"??_);_(@_)";
        }

        public static void SetExcelNumberFormat2(ExcelRange range)
        {
            range.Style.Numberformat.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* \" - \"??_);_(@_)";
            //range.Style.Numberformat.Format = "#,##0";
        }

        public static void SetExcelNumberFormatUsd2NotHide0(ExcelRange range)
        {
            range.Style.Numberformat.Format = "$ #,##0.00";
        }

        public static void SetExcelNumberFormatNotHide0(ExcelRange range)
        {
            range.Style.Numberformat.Format = "#,##0_);(#,##0)";
        }

        public static void SetExcelNumberFormat2NotHide0(ExcelRange range)
        {
            range.Style.Numberformat.Format = "#,##0.00";
        }

        // 0.00%
        public static void SetExcelTimeFormat(ExcelRange range)
        {
            range.Style.Numberformat.Format = "hh:mm;@";
        }

        public static void SetExcelDateFormat(ExcelRange range)
        {
            range.Style.Numberformat.Format = "dd/mm/yyyy;@";
        }

        public static void SetExcelDateTimeFormat(ExcelRange range)
        {
            range.Style.Numberformat.Format = "dd/mm/yyyy hh:mm;@";
        }

        public static string GetExcelDateTimeRangeTitle(DateTime from, DateTime to)
        {
            var res = "";

            var dayOfWeekVietnamese = new Hashtable();
            dayOfWeekVietnamese.Add("Monday", "Thứ 2");
            dayOfWeekVietnamese.Add("Tuesday", "Thứ 3");
            dayOfWeekVietnamese.Add("Wednesday", "Thứ 4");
            dayOfWeekVietnamese.Add("Thursday", "Thứ 5");
            dayOfWeekVietnamese.Add("Friday", "Thứ 6");
            dayOfWeekVietnamese.Add("Saturday", "Thứ 7");
            dayOfWeekVietnamese.Add("Sunday", "Chủ nhật");

            //dayOfWeekVietnamese.Add("Monday", "Thứ hai");
            //dayOfWeekVietnamese.Add("Tuesday", "Thứ ba");
            //dayOfWeekVietnamese.Add("Wednesday", "Thứ tư");
            //dayOfWeekVietnamese.Add("Thursday", "Thứ năm");
            //dayOfWeekVietnamese.Add("Friday", "Thứ sáu");
            //dayOfWeekVietnamese.Add("Saturday", "Thứ bẩy");
            //dayOfWeekVietnamese.Add("Sunday", "Chủ nhật");

            var title = "Từ {0} ngày {1} đến {2} ngày {3}";
            res = string.Format(title, dayOfWeekVietnamese[from.DayOfWeek.ToString()], from.ToString("dd/MM/yyyy"), dayOfWeekVietnamese[to.DayOfWeek.ToString()], to.ToString("dd/MM/yyyy"));

            return res;
        }

        public static string GetVietnameseDayOfWeek(DateTime date, bool returnName = true)
        {
            var dayOfWeekVietnamese1 = new Hashtable();
            dayOfWeekVietnamese1.Add("Monday", "Thứ 2");
            dayOfWeekVietnamese1.Add("Tuesday", "Thứ 3");
            dayOfWeekVietnamese1.Add("Wednesday", "Thứ 4");
            dayOfWeekVietnamese1.Add("Thursday", "Thứ 5");
            dayOfWeekVietnamese1.Add("Friday", "Thứ 6");
            dayOfWeekVietnamese1.Add("Saturday", "Thứ 7");
            dayOfWeekVietnamese1.Add("Sunday", "Chủ nhật");

            var dayOfWeekVietnamese2 = new Hashtable();
            dayOfWeekVietnamese2.Add("Monday", "Thứ hai");
            dayOfWeekVietnamese2.Add("Tuesday", "Thứ ba");
            dayOfWeekVietnamese2.Add("Wednesday", "Thứ tư");
            dayOfWeekVietnamese2.Add("Thursday", "Thứ năm");
            dayOfWeekVietnamese2.Add("Friday", "Thứ sáu");
            dayOfWeekVietnamese2.Add("Saturday", "Thứ bảy");
            dayOfWeekVietnamese2.Add("Sunday", "Chủ nhật");

            return returnName ? dayOfWeekVietnamese2[date.DayOfWeek.ToString()].ToString() : dayOfWeekVietnamese1[date.DayOfWeek.ToString()].ToString();
        }

        public static void SetExcelPercentFormat(ExcelRange range)
        {
            range.Style.Numberformat.Format = "0%;-0%;\" \"";
        }

        public static void SetExcelPercentFormat1(ExcelRange range)
        {
            range.Style.Numberformat.Format = "0.0%;-0.0%;\" \"";
        }

        public static void SetExcelPercentFormat2(ExcelRange range)
        {
            range.Style.Numberformat.Format = "0.00%;-0.00%;\" \"";
        }
    }
}
