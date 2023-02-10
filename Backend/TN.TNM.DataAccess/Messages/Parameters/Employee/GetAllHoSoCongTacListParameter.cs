
using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllHoSoCongTacListParameter : BaseParameter
    {
        public string MaHoSo { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public int? TrangThai { get; set; }
    }
}
