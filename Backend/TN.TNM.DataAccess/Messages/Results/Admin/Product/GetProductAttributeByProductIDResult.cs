using System.Collections.Generic;
using TN.TNM.DataAccess.Models.ProductAttributeCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetProductAttributeByProductIDResult : BaseResult
    {
        public List<ProductAttributeCategoryEntityModel> lstProductAttributeCategory { get; set; }
    }
}
