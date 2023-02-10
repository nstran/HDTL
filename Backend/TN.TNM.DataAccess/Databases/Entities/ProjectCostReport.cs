using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectCostReport
    {
        public Guid ProjectCostReportId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Pv { get; set; }
        public decimal? Ac { get; set; }
        public decimal? Ev { get; set; }
        public bool Active { get; set; }
    }
}
