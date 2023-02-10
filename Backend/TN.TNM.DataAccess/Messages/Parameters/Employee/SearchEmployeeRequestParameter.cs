using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SearchEmployeeRequestParameter : BaseParameter
    {
        public string Code { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public Guid? OrganizationId { get; set; }
        public List<int> ListLoaiDeXuatId { get; set; }
        public List<int> ListStatusId { get; set; }
    }
}
