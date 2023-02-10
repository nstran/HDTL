using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetThongTinNhanSuResponse : BaseResponse
    {
        public ThongTinNhanSuModel ThongTinNhanSu { get; set; }
    }
}
