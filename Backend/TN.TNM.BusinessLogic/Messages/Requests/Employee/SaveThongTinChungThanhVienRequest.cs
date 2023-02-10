using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SaveThongTinChungThanhVienRequest : BaseRequest<SaveThongTinChungThanhVienParameter>
    {
        public ThongTinChungThanhVienModel ThongTinChungThanhVien { get; set; }
        public override SaveThongTinChungThanhVienParameter ToParameter()
        {
            return new SaveThongTinChungThanhVienParameter()
            {
                UserId = UserId,
                ThongTinChungThanhVien = ThongTinChungThanhVien
            };
        }
    }
}
