using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class SearchTimeSheetRequest : BaseRequest<SearchTimeSheetParameter>
    {
        public Guid ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<Guid> ListStatusId { get; set; }
        public List<Guid> ListTimeTypeId { get; set; }
        public List<Guid> ListPersionInChargedId { get; set; }
        public bool IsShowAll { get; set; }
        public override SearchTimeSheetParameter ToParameter()
        {
            return new SearchTimeSheetParameter
            {
                ProjectId = this.ProjectId,
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                ListStatusId = this.ListStatusId,
                ListTimeTypeId = this.ListTimeTypeId,
                ListPersionInChargedId = this.ListPersionInChargedId,
                IsShowAll = this.IsShowAll,
                UserId = this.UserId
            };
        }
    }
}
