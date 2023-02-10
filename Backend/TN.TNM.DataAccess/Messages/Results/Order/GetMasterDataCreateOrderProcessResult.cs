using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataCreateOrderProcessResult: BaseResult
    {
        public List<ProductCategoryEntityModel> ListServiceType { get; set; }
        public List<ServicePacketEntityModel> ListServicePacket { get; set; }
        public List<CustomerEntityModel> ListCustomer { get; set; }
        public List<PermissionConfigurationEntityModel> ListProcessOfPack { get; set; }

        
    }
}
