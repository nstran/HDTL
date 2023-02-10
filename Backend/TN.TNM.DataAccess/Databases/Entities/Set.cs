using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Set
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public double Score { get; set; }
        public string Value { get; set; }
        public DateTime? Expireat { get; set; }
        public int Updatecount { get; set; }
    }
}
