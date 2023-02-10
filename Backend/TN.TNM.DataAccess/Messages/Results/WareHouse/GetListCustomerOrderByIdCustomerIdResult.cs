using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Order;
using Entities = TN.TNM.DataAccess.Databases.Entities;


namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetListCustomerOrderByIdCustomerIdResult:BaseResult
    {
        public List<CustomerOrderEntityModel> listCustomerOrder { get; set; }
    }
}
