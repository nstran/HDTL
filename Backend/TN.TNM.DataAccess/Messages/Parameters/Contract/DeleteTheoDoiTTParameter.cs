using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Contract
{
    public class DeleteTheoDoiTTParameter: BaseParameter
    {
        public Guid TheoDoiThanhToanId { get; set; }
    }
}
