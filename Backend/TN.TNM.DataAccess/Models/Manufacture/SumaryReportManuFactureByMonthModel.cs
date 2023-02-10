using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class SumaryReportManuFactureByMonthModel
    {
        public double? Quantity { get; set; }//số tấm
        public double? TotalAreaThick { get; set; }//số m2 dày
        public double? TotalAreaThin { get; set; }//số m2 mỏng
        public double? TotalAreaEspeciallyThick { get; set; }//số m2 đặc biệt dày
        public double? TotalAreaEspeciallyThin { get; set; }//số m2 đặc biệt mỏng
        public double? TotalAreaBoreholeThick { get; set; } //số m2 khoan khoét dày
        public double? TotalAreaBoreholeThin { get; set; } //số m2 khoan khoét mỏng
        public double? TotalAreaOriginalThick { get; set; }//số m2 nguyên khối dày 
        public double? TotalAreaOriginalThin { get; set; }//số m2 nguyên khối mỏng
        public double? TotalDayShift { get; set; } //tổng ngày
        public double? TotalNightShift { get; set; } //tổng đêm
        public double? TotalArea { get; set; } //tổng sản lượng

        public SumaryReportManuFactureByMonthModel()
        {
            this.Quantity = 0;
            this.TotalAreaThick = 0;
            this.TotalAreaThin = 0;
            this.TotalAreaEspeciallyThick = 0;
            this.TotalAreaEspeciallyThin = 0;
            this.TotalAreaBoreholeThick = 0;
            this.TotalAreaBoreholeThin = 0;
            this.TotalAreaOriginalThick = 0;
            this.TotalAreaOriginalThin = 0;
            this.TotalDayShift = 0;
            this.TotalNightShift = 0;
            this.TotalArea = 0;
        }
    }
}
