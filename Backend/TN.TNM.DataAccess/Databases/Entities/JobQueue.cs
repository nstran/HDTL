using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Jobqueue
    {
        public long Id { get; set; }
        public long Jobid { get; set; }
        public string Queue { get; set; }
        public DateTime? Fetchedat { get; set; }
        public int Updatecount { get; set; }
    }
}
