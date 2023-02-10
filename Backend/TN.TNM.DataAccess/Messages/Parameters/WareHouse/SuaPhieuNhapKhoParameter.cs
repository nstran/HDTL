using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class SuaPhieuNhapKhoParameter : BaseParameter
    {
        public InventoryReceivingVoucherEntityModel InventoryReceivingVoucher { get; set; }
        public List<InventoryReceivingVoucherMapping> ListInventoryReceivingVoucherMapping { get; set; }
    }
}
