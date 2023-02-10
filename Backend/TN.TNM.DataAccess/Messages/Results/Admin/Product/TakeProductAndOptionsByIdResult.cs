using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class TakeProductAndOptionsByIdResult : BaseResult
    {
        public ProductEntityModel ProductEntityModel { get; set; }
    }
}
