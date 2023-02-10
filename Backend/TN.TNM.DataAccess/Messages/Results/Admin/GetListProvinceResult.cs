using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class GetListProvinceResult : BaseResult
    {
        public List<ProvinceEntityModel> ListProvinceEntityModel { get; set; }
    }
}
