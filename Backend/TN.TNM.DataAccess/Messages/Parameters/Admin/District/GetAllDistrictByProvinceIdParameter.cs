using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.District
{
    public class GetAllDistrictByProvinceIdParameter : BaseParameter
    {
        public Guid ProvinceId { get; set; }
    }
}
