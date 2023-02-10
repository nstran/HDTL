using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Manufacture;

namespace TN.TNM.DataAccess.Messages.Results.Manufacture
{
    public class GetMasterDataTrackProductionResult : BaseResult
    {
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string CurrentTime { get; set; }
        public List<CategoryEntityModel> ListStatusItem { get; set; }
        public List<ProductionOrderEntityModel> ListProductionOrder { get; set; }
    }
}
