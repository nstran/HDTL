using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Contact
{
    public class GetAddressByChangeObjectResult : BaseResult
    {
        public List<DistrictEntityModel> ListDistrict { get; set; }
        public List<WardEntityModel> ListWard { get; set; }
    }
}
