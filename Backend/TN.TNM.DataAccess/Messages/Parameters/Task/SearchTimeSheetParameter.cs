using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class SearchTimeSheetParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<Guid> ListStatusId { get; set; }
        public List<Guid> ListTimeTypeId { get; set; }
        public List<Guid> ListPersionInChargedId { get; set; }
        public bool IsShowAll { get; set; }
    }
}
