using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Admin.District
{
    public class GetAllDistrictByProvinceIdResult : BaseResult
    {
        public List<DistrictEntityModel> ListDistrict { get; set; }
    }
}
