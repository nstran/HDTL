using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class SynchonizedHistory
    {
        public int Id { get; set; }
        public DateTime? SynchonizedTime { get; set; }
        public bool? SyncStatus { get; set; }
        public string Note { get; set; }
    }
}
