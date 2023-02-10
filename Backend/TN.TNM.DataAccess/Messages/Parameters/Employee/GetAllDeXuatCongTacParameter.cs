using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetAllDeXuatCongTacParameter : BaseParameter
    {
        public string MaDeXuat { get; set; }
        public List<Guid> ListEmployee { get; set; }
        public int? TrangThai { get; set; }
    }
}
