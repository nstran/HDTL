using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GanMucDanhGiaChungParameter: BaseParameter
    {
        public List<NhanVienKyDanhGiaEntityModel> ListNhanVien { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
    }
}
