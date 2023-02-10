using System.Collections.Generic;
using TN.TNM.DataAccess.Models.WareHouse;
using Entities = TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.WareHouse
{
    public class GetWareHouseChaResult : BaseResult
    {
        public List<WareHouseEntityModel> listWareHouse { get; set; }
    }
}
