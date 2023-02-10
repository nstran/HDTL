using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SaveThongTinCaNhanThanhVienRequest : BaseRequest<SaveThongTinCaNhanThanhVienParameter>
    {
        public ThongTinCaNhanThanhVienModel ThongTinCaNhanThanhVien { get; set; }
        public override SaveThongTinCaNhanThanhVienParameter ToParameter()
        {
            return new SaveThongTinCaNhanThanhVienParameter()
            {
                UserId = UserId,
                ThongTinCaNhanThanhVien = ThongTinCaNhanThanhVien
            };
        }
    }
}
