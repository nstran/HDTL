using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    public class SumaryDashboardEntityModel
    {
        public double? TotalAreaHasToComplate { get; set; } //tổng phải hoàn thành
        public double? TotalAreaCompleted { get; set; } // đã hoàn thành
        public double? TotalAreaRemain { get; set; } // còn lại
        public double? TotalAreaCompletedByPercent { get; set; } // đã hoàn thành (theo phần trăm)
        public double? TotalAreaRemainByPercent { get; set; } // còn lại  (theo phần trăm)
        
        public SumaryDashboardEntityModel()
        {
            this.TotalAreaHasToComplate = 0;
            this.TotalAreaCompleted = 0;
            this.TotalAreaRemain = 0;
            this.TotalAreaCompletedByPercent = 0;
            this.TotalAreaRemainByPercent = 100; 
        }
    }
}
