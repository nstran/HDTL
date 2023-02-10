using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Salary
{
    public class DataChamCongConvertModel
    {
        public Guid EmployeeId { get; set; }
        public DateTime NgayChamCong { get; set; }
        public TimeSpan ThoiGianChamCong { get; set; }
    }
}
