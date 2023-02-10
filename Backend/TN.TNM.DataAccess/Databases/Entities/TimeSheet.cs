using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TimeSheet
    {
        public Guid TimeSheetId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? PersonInChargeId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? SpentHour { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? Status { get; set; }
        public Guid? TimeType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
