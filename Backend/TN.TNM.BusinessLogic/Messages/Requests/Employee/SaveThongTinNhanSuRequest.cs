using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SaveThongTinNhanSuRequest : BaseRequest<SaveThongTinNhanSuParameter>
    {
        public ThongTinNhanSuModel ThongTinNhanSu { get; set; }
        public override SaveThongTinNhanSuParameter ToParameter()
        {
            return new SaveThongTinNhanSuParameter()
            {
                UserId = UserId,
                ThongTinNhanSu = ThongTinNhanSu
            };
        }
    }
}
