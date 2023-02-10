using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Manufacture
{
    //model phục vụ cho việc xuất báo cáo sai hỏng theo tổ sản xuất
    public class TechniqueRequestReportModel
    {
        public Guid? TechniqueRequestId { get; set; }
        public string TechniqueName { get; set; }
        public double? TotalArea { get; set; } //tổng sản lượng
        public double? TotalAreaError { get; set; } //tổng lỗi
        public double? TotalAreaErrorByThickGlass { get; set; }//tổng lỗi theo kính dày
        public double? TotalAreaErrorByThinGlass { get; set; }//tổng lỗi theo kính mỏng
        public double? TotalAreaErrorByTechEquipments { get; set; }//tổng lỗi theo thiết bị công nghệ
        public double? TotalAreaErrorByManufacture { get; set; }//tổng lỗi theo tổ sản xuất
        public double? TotalAreaErrorByMaterials { get; set; }//tổng lỗi theo vật tư

        public double? TotalAreaByPercent { get; set; } //tổng sản lượng theo phần trăm
        public double? TotalAreaErrorByPercent { get; set; } //tổng lỗi theo phần trăm
        public double? TotalAreaErrorByThickGlassByPercent { get; set; }//tổng lỗi theo kính dày theo phần trăm
        public double? TotalAreaErrorByThinGlassByPercent { get; set; }//tổng lỗi theo kính mỏng theo phần trăm
        public double? TotalAreaErrorByTechEquipmentsByPercent { get; set; }//tổng lỗi theo thiết bị công nghệ theo phần trăm
        public double? TotalAreaErrorByManufactureByPercent { get; set; }//tổng lỗi theo tổ sản xuất theo phần trăm
        public double? TotalAreaErrorByMaterialsByPercent { get; set; }//tổng lỗi theo vật tư theo phần trăm
    }
}
