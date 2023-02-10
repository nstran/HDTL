using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Options;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductMappingOptions;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateOrEditProductParameter : BaseParameter
    {
        public ProductEntityModel ProductEntityModel { get; set; }

        public List<OptionsEntityModel> ListOptionsEntityModel { get; set; }
    }
}
