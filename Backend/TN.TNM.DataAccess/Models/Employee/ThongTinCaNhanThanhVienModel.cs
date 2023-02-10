using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinCaNhanThanhVienModel
    {
        public Guid? EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public double SoBuoiNghiCoPhep { get; set; }
        public double SoBuoiNghiKhongPhep { get; set; }
        public double SoBuoiPhepNam { get; set; }
        public double SoBuoiPhepCon { get; set; }
    }
}
