using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetLichSuHopDongNhanSuParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public List<int> ListId { get; set; }
    }
}
