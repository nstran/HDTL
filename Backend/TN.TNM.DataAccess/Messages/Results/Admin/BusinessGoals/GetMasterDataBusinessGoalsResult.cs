using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BusinessGoals;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.BusinessGoals
{
    public class GetMasterDataBusinessGoalsResult : BaseResult
    {
        //public List<ProductEntityModel> ListProduct { get; set; }
        public List<ProductCategoryEntityModel> ListProductCategory { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }

        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsSalesDetail { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsRevenueDetail { get; set; }

        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsSalesDetailChild { get; set; }
        public List<BusinessGoalsDetailEntityModel> ListBusinessGoalsRevenueDetailChild { get; set; }

    }
}
