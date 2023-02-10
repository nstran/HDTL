using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class SumaryReportManuFactureByDayModel
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
        public double? TotalArea { get; set; }//số m2 tất cả
    }
}
