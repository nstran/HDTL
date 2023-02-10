using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateChamCongParameter : BaseParameter
    {
        public string TypeUpdate { get; set; }
        public int TypeField { get; set; }
        public List<Guid> ListEmployeeId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public TimeSpan? ThoiGian { get; set; }
        public int? MaKyHieu { get; set; }
    }
}
