using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class ImportHDNSParameter: BaseParameter
    {
        public List<HopDongNhanSuModel> ListHopDong { get; set; }
        //public Guid EmployeeCode { get; set; }
    }
}
