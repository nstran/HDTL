using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class SearchTaskFromProjectScopeRequest : BaseRequest<SearchTaskParameter>
    {
        public Guid ProjectId { get; set; }
        public List<Guid?> ListStatusId { get; set; }
        public List<Guid?> ListEmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid?> ListWorkpackageId { get; set; }
        public DateTime? FromEndDate { get; set; }
        public DateTime? ToEndDate { get; set; }
        public int FromPercent { get; set; }
        public int ToPercent { get; set; }
        public override SearchTaskParameter ToParameter()
        {
            return new SearchTaskParameter
            {
                UserId = this.UserId,
                ProjectId = this.ProjectId,
                ListStatusId = this.ListStatusId,
                ListEmployeeId = this.ListEmployeeId,
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                ListWorkpackageId = this.ListWorkpackageId,
                FromEndDate = this.FromEndDate,
                ToEndDate = this.ToEndDate,
                FromPercent = this.FromPercent,
                ToPercent = this.ToPercent,
            };
        }
    }
}
