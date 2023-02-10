using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class SaveLuongCtTroCapKhacParameter : BaseParameter
    {
        public int KyLuongId { get; set; }
        public List<Guid> ListLoaiTroCapKhacId { get; set; }
    }
}
