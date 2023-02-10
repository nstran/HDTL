using TN.TNM.DataAccess.Models.WareHouse;

namespace TN.TNM.DataAccess.Messages.Parameters.WareHouse
{
    public class CreateUpdateWareHouseParameter : BaseParameter
    {
        public WareHouseEntityModel Warehouse { get; set; }
    }
}
