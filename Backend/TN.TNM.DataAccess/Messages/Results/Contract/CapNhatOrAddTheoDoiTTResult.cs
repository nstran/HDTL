using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Contract;

namespace TN.TNM.DataAccess.Messages.Results.Contract
{
    public class CapNhatOrAddTheoDoiTTResult: BaseResult
    {
        public TheoDoiThanhToanEntityModel TheoDoiThanhToan { get; set; }
    }
}
