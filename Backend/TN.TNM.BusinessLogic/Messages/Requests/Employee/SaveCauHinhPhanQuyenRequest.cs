using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Requests.Employee
{
    public class SaveCauHinhPhanQuyenRequest : BaseRequest<SaveCauHinhPhanQuyenParameter>
    {
        public Guid EmployeeId { get; set; }
        public bool IsManager { get; set; }
        public Guid? RoleId { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }
        public override SaveCauHinhPhanQuyenParameter ToParameter()
        {
            return new SaveCauHinhPhanQuyenParameter()
            {
                UserId = UserId,
                EmployeeId = EmployeeId,
                IsManager = IsManager,
                RoleId = RoleId,
                ListThanhVienPhongBan = ListThanhVienPhongBan
            };
        }
    }
}
