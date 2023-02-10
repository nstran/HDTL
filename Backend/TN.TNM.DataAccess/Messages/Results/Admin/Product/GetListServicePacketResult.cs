using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetListServicePacketResult : BaseResult
    {
        public List<ServicePacketEntityModel> ListServicePackageEntityModel { get; set; }
    }
}
