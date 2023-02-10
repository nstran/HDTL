using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateChamCongOtParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public Guid LoaiOtId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public string Type { get; set; }
        public TimeSpan? ThoiGian { get; set; } 
    }
}
