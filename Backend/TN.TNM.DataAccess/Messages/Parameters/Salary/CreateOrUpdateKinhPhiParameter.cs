using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateKinhPhiParameter : BaseParameter
    {
        public KinhPhiCongDoanModel KinhPhiCongDoan { get; set; }
    }
}
