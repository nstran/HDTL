using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.OT;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class PheDuyetNhanSuDangKyOTParameter: BaseParameter
    {
        public List<KeHoachOtThanhVienEntityModel> ListNv { get; set; }
        public List<Guid> ListNvPheDuyetID { get; set; }
        public Guid OrganizationId { get; set; }
        public int KeHoachOtId { get; set; } 
    }
}
