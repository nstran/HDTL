using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class ImportTroCapKhacModel
    {
        public string EmployeeCode { get; set; }
        public List<ImportLoaiTroCapKhacModel> ListDataLoaiTroCap { get; set; }
    }

    public class ImportLoaiTroCapKhacModel
    {
        public string TenLoaiTroCap { get; set; }
        public decimal MucTroCap { get; set; }
    }
}
