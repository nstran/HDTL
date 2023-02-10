using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class LuuOrHoanThanhDanhGiaParameter: BaseParameter
    {
        public List<ChiTietDanhGiaNhanVienEntityModel> ListCauTraLoi { get; set; }
        public DanhGiaNhanVien DanhGiaNhanVien { get; set; }
        public bool? IsClickHoanThanh { get; set; }
    }
}
