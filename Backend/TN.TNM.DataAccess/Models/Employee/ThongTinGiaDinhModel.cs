using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinGiaDinhModel
    {
        public Guid EmployeeId { get; set; }
        public Guid? ContactId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public Guid QuanHeId { get; set; }
        public bool? PhuThuoc { get; set; }
        public DateTime? PhuThuocTuNgay { get; set; }
        public DateTime? PhuThuocDenNgay { get; set; }
    }
}
