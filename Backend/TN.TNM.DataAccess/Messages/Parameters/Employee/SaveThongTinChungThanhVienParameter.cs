using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.File;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SaveThongTinChungThanhVienParameter : BaseParameter
    {
        public ThongTinChungThanhVienModel ThongTinChungThanhVien { get; set; }
        public List<Guid> ListPhongBanId { get; set; }
        public FileBase64Model FileBase64 { get; set; }
    }
}
