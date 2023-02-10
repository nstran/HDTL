using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.Employee
{
    public class GetCauHinhPhanQuyenResponse : BaseResponse
    {
        public Guid RoleId { get; set; }
        public bool IsManager { get; set; }
        public List<ThanhVienPhongBanModel> ListSelectedDonVi { get; set; }
    }
}
