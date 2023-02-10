using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SaveThongTinLuongVaTroCapRequest : BaseRequest<SaveThongTinLuongVaTroCapParameter>
    {
        public ThongTinLuongVaTroCapModel ThongTinLuongVaTroCap { get; set; }
        public override SaveThongTinLuongVaTroCapParameter ToParameter()
        {
            return new SaveThongTinLuongVaTroCapParameter()
            {
                UserId = UserId,
                ThongTinLuongVaTroCap = ThongTinLuongVaTroCap
            };
        }
    }
}
