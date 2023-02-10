using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Jobparameter
    {
        public long Id { get; set; }
        public long Jobid { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Updatecount { get; set; }

        public Job Job { get; set; }
    }
}
