using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetThongTinLuongVaTroCapResponse : BaseResponse
    {
        public ThongTinLuongVaTroCapModel ThongTinLuongVaTroCap { get; set; }
    }
}
