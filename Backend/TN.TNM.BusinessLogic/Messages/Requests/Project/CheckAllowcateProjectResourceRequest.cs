using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class CheckAllowcateProjectResourceRequest : BaseRequest<CheckAllowcateProjectResourceParameter>
    {
        public Guid ResourceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Allowcation { get; set; }
        public Guid ProjectResourceId { get; set; }
        public override CheckAllowcateProjectResourceParameter ToParameter()
        {
            return new CheckAllowcateProjectResourceParameter()
            {
                ResourceId = ResourceId,
                Allowcation = Allowcation,
                FromDate = FromDate,
                ToDate = ToDate,
                ProjectResourceId = ProjectResourceId
            };
        }
    }
}
