using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataListSearchProductionOrderResult : BaseResult
    {
        public List<Category> ListStatus { get; set; }
        public List<Organization> Organizations { get; set; }
    }
}
