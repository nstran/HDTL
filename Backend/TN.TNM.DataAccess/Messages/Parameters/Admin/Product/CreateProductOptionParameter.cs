using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateProductOptionParameter : BaseParameter
    {
        public OptionsEntityModel OptionsEntityModel { get; set; }
    }
}
