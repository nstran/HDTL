using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.BusinessGoals;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals
{
    public class CreateOrUpdateBusinessGoalsParameter : BaseParameter
    {
        public BusinessGoalsEntityModel BusinessGoals { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsDetailSales { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsDetailRevenue { get; set; }
    }
}
