using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DashboardHomeViewDetailParameter: BaseParameter
    {
        public int Type { get; set; }
        public int? Month { get; set; }
    }
}
