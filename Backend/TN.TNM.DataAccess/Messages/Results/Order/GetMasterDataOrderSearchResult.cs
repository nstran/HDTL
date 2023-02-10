using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Contract;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderSearchResult : BaseResult
    {
        public List<ServicePacketEntityModel> ListPacketService { get; set; }
        public List<EmployeeEntityModel> ListCreator { get; set; }
        public List<TrangThaiGeneral> ListStatus { get; set; }
        public List<CustomerEntityModel> ListCus { get; set; }
        public List<ProductCategoryEntityModel> ListAllProductCategory { get; set; }
        
    }
}
