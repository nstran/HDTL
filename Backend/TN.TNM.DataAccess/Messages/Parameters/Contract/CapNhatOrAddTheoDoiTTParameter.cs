using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class CapNhatOrAddTheoDoiTTParameter: BaseParameter
    {
        public TheoDoiThanhToanEntityModel TheoDoiTT { get; set; }
    }
}
