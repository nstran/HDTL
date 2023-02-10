using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class GetChamCongOtByEmpIdParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
}
