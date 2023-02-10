using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class ThongTinKhacNhanSuModel
    {
        public Guid EmployeeId { get; set; }
        public string BienSo { get; set; }
        public string LoaiXe { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankOwnerName { get; set; }
        public string BankAccount { get; set; }
    }
}
