using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Task
{
    public class TimeSheetDetailEntityModel
    {
        public Guid TimeSheetDetailId { get; set; }
        public Guid TimeSheetId { get; set; }
        public DateTime? Date { get; set; }
        public decimal? SpentHour { get; set; }
        public int? Status { get; set; }
    }
}
