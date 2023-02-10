using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.BusinessGoals
{
    public class GetMasterDataBusinessGoalsRequest : BaseRequest<GetMasterDataBusinessGoalsParameter>
    {
        public Guid? OrganizationId { get; set; }
        public string Year { get; set; }
        public bool Type { get; set; }
        public override GetMasterDataBusinessGoalsParameter ToParameter()
        {
            return new GetMasterDataBusinessGoalsParameter
            {
                OrganizationId = this.OrganizationId,
                Year = this.Year,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
