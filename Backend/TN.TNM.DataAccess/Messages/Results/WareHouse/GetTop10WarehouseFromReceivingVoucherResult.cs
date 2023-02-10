
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetTop10WarehouseFromReceivingVoucherResult:BaseResult
    {
       public List<InventoryReceivingVoucherMappingEntityModel> lstInventoryReceivingVoucherMapping { get; set; }
    }
}
