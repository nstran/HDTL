using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetCauHinhPhanQuyenResult : BaseResult
    {
        public Guid RoleId { get; set; }
        public bool IsManager { get; set; }
        public List<ThanhVienPhongBanModel> ListSelectedDonVi { get; set; }
        public bool IsShowButtonSua { get; set; }
    }
}
