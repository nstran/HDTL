using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class SearchTaskRequest : BaseRequest<SearchTaskParameter>
    {
        public Guid ProjectId { get; set; }

        public List<Guid?> ListTaskTypeId { get; set; }
        public List<Guid?> ListStatusId { get; set; }
        public List<int> ListPriority { get; set; }
        public List<Guid?> ListEmployeeId { get; set; }

        // Ngày hết hạn
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string OptionStatus { get; set; }
        public string Type { get; set; }

        public override SearchTaskParameter ToParameter()
        {
            return new SearchTaskParameter
            {
                ProjectId = this.ProjectId,
                ListTaskTypeId = this.ListTaskTypeId,
                ListStatusId = this.ListStatusId,
                ListPriority = this.ListPriority,
                ListEmployeeId = this.ListEmployeeId,
                FromDate = this.FromDate,
                ToDate = this.ToDate,
                OptionStatus = this.OptionStatus,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
