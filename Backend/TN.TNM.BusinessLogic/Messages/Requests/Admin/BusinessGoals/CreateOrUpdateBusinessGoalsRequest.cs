using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals;
using TN.TNM.DataAccess.Models.BusinessGoals;

namespace TN.TNM.BusinessLogic.Messages.Requests.Admin.BusinessGoals
{
    public class CreateOrUpdateBusinessGoalsRequest : BaseRequest<CreateOrUpdateBusinessGoalsParameter>
    {
        public BusinessGoalsEntityModel BusinessGoals { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsDetailSales { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsDetailRevenue { get; set; }

        public override CreateOrUpdateBusinessGoalsParameter ToParameter()
        {
            return new CreateOrUpdateBusinessGoalsParameter
            {
                BusinessGoals = this.BusinessGoals,
                ListBusinessGoalsDetailRevenue = this.ListBusinessGoalsDetailRevenue,
                ListBusinessGoalsDetailSales = this.ListBusinessGoalsDetailSales
            };
        }
    }
}
