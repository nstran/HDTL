using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SaveCauHinhPhanQuyenParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public bool IsManager { get; set; }
        public Guid? RoleId { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }
    }
}
