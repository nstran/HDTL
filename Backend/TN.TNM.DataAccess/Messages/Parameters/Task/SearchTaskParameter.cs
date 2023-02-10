using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class SearchTaskParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }

        public List<Guid?> ListTaskTypeId { get; set; }
        public List<Guid?> ListStatusId { get; set; }
        public List<int> ListPriority { get; set; }
        public List<Guid?> ListEmployeeId { get; set; }
        public List<Guid?> ListCreatedId { get; set; }

        // Ngày hết hạn
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string Type { get; set; }
        public string OptionStatus { get; set; }
        public List<Guid?> ListWorkpackageId { get; set; }
        public DateTime? FromEndDate { get; set; }
        public DateTime? ToEndDate { get; set; }

        public int FromPercent { get; set; }
        public int ToPercent { get; set; }
    }
}
