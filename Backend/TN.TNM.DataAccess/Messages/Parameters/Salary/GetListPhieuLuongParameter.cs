using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class GetListPhieuLuongParameter : BaseParameter
    {
        public List<int> ListKyLuongId { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
    }
}
