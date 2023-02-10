
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllDenghiTamUngListParameter : BaseParameter
    {
        public string MaDenghi{ get; set; }
        public List<Guid> ListEmployee { get; set; }
        public int? TrangThai { get; set; }
        public int? LoaiDeNghi { get; set; }
    }
}
