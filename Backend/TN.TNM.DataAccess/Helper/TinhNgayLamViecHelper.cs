using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Helper
{
    public static class TinhNgayLamViecHelper
    {
        private const int MONDAY = 1;
        private const int TUESDAY = 2;
        private const int WEDNESDAY = 3;
        private const int THURSDAY = 4;
        private const int FRIDAY = 5;
        private const int SATURDAY = 6;
        private const int SUNDAY = 7;

        public static void GetWeekendDaysBetween(DateTime startDate, DateTime endDate, bool skip, out int saturdays, out int sundays)
        {
            saturdays = 0;
            sundays = 0;
            if (endDate < startDate || skip)
                return;

            #region Cách tính 1

            //TimeSpan timeBetween = endDate.Subtract(startDate);
            //int weekendsBetween = timeBetween.Days / 7;
            //sundays = weekendsBetween;
            //saturdays = weekendsBetween;
            //int startDay = GetDayOfWeekNumber(startDate.DayOfWeek);
            //int endDay = GetDayOfWeekNumber(endDate.DayOfWeek);
            //if (startDay > endDay)
            //{
            //    sundays++;
            //    saturdays += (startDay < SUNDAY) ? 1 : 0;
            //}
            //else if (startDay < endDay)
            //{
            //    saturdays += (endDay == SATURDAY || endDay == SUNDAY) ? 1 : 0;
            //}
            //else if (startDay == endDay)
            //{
            //    if (startDay == SATURDAY)
            //        saturdays++;
            //    if (startDay == SUNDAY)
            //        sundays++;
            //}

            #endregion

            #region Cách tính 2

            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.Date.AddDays(1))
            {
                if (date.Date.DayOfWeek == DayOfWeek.Saturday)
                    saturdays++;

                if (date.Date.DayOfWeek == DayOfWeek.Sunday)
                    sundays++;
            }

            #endregion
        }

        public static void GetSoNgayNghiPhep(DateTime startDate, DateTime endDate, 
            Guid statusNghiPhep, Guid statusNghiKhongLuong, List<EmployeeRequest> listDeXuatXinNghi, 
            out decimal soNgayNghiPhep, out decimal soNgayNghiKhongLuong)
        {
            decimal _soNgayNghiPhep = 0;
            decimal _soNgayNghiKhongLuong = 0;

            listDeXuatXinNghi.ForEach(empR =>
            {
                for (var date = empR.StartDate.Value.Date; 
                    (date <= empR.EnDate.Value.Date && date >= startDate.Date && date <= endDate.Date); 
                    date = date.AddDays(1))
                {
                    if (empR.TypeRequest == statusNghiPhep)
                    {
                        if (empR.StartTypeTime == empR.EndTypeTime)
                        {
                            _soNgayNghiPhep += 1 / 2;
                        }
                        else
                        {
                            _soNgayNghiPhep += 1;
                        }
                    }

                    if (empR.TypeRequest == statusNghiKhongLuong)
                    {
                        if (empR.StartTypeTime == empR.EndTypeTime)
                        {
                            _soNgayNghiKhongLuong += 1 / 2;
                        }
                        else
                        {
                            _soNgayNghiKhongLuong += 1;
                        }
                    }
                }
            });

            soNgayNghiPhep = _soNgayNghiPhep;
            soNgayNghiKhongLuong = _soNgayNghiKhongLuong;
        }

        private static int GetDayOfWeekNumber(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return MONDAY;
                case DayOfWeek.Tuesday:
                    return TUESDAY;
                case DayOfWeek.Wednesday:
                    return WEDNESDAY;
                case DayOfWeek.Thursday:
                    return THURSDAY;
                case DayOfWeek.Friday:
                    return FRIDAY;
                case DayOfWeek.Saturday:
                    return SATURDAY;
                case DayOfWeek.Sunday:
                    return SUNDAY;
                default:
                    throw new ArgumentException("Invalid day!");
            }
        }
    }
}
