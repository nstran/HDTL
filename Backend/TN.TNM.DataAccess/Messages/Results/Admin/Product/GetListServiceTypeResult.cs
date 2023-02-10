using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetListServiceTypeResult : BaseResult
    {
        public List<CategoryEntityModel> ListProductCategory { get; set; }

    }
}
