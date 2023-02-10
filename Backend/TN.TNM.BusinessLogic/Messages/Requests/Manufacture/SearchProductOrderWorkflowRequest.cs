using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class SearchProductOrderWorkflowRequest : BaseRequest<SearchProductOrderWorkflowParameter>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }
        public override SearchProductOrderWorkflowParameter ToParameter()
        {
            return new SearchProductOrderWorkflowParameter()
            {
                UserId = UserId,
                Code = Code,
                Name = Name,
                Active = Active,
                Description = Description
            };
        }
    }
}
