using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class CheckAllowcateProjectResourceParameter : BaseParameter
    {
        public Guid ResourceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Allowcation { get; set; }
        public Guid ProjectResourceId { get; set; }
    }
}
