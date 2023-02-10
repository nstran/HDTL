using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinLuongVaTroCapModel
    {
        public Guid? EmployeeId { get; set; }
        public List<EmployeeSalaryModel> ListEmployeeSalary { get; set; }
        public decimal? EmployeeSalaryBase { get; set; } //Lương cơ bản
        public DateTime? EffectiveDate { get; set; } //Ngày hiệu lực
        public decimal? LunchAllowance { get; set; } //Ăn trưa
        public decimal? MaternityAllowance { get; set; } //Phép năm
        public decimal? FuelAllowance { get; set; } //Xăng xe
        public decimal? PhoneAllowance { get; set; } //Điện thoại
        public decimal? OtherAllownce { get; set; } //Trợ cấp khác
        public decimal? SocialInsuranceSalary { get; set; } //Lương đóng BHXH
        public decimal? SocialInsuranceSupportPercent { get; set; } //Mức hỗ trợ BHXH
        public decimal? SocialInsurancePercent { get; set; } //Mức hỗ trợ BHTN
        public decimal? HealthInsuranceSupportPercent { get; set; } //Mức hỗ trợ BHYT
        public decimal? UnemploymentinsuranceSupportPercent { get; set; } //Mức đóng BHXH
        public decimal? HealthInsurancePercent { get; set; } //Mức đóng BHTN
        public decimal? UnemploymentinsurancePercent { get; set; } //Mức đóng BHYT
        public decimal ChiPhiTheoGio { get; set; }
    }
}
