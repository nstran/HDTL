using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Organization
{
    public class UpdateOrganizationByIdParameter : BaseParameter
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationPhone { get; set; }
        public string OrganizationAddress { get; set; }
        public bool IsFinancialIndependence { get; set; }
        public Guid? GeographicalAreaId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? SatelliteId { get; set; }
        public string OrganizationOtherCode { get; set; }
        public bool IsAccess { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }
    }
}
