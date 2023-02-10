using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetThongTinChungThanhVienResponse : BaseResponse
    {
        public ThongTinChungThanhVienModel ThongTinChung { get; set; }
    }
}
