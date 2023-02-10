using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.BusinessLogic.Messages.Responses.Contact
{
    public class GetAddressByContactIdResponse : BaseResponse
    {
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<DistrictEntityModel> ListDistrict { get; set; }
        public List<WardEntityModel> ListWard { get; set; }
    }
}
