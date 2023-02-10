using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TimeSheetDetail
    {
        public Guid TimeSheetDetailId { get; set; }
        public Guid TimeSheetId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? SpentHour { get; set; }
        public int? Status { get; set; }
        public Guid? TenantId { get; set; }
    }
}
