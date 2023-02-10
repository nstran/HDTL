using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class GetMasterDataOrderDetailDialogResult : BaseResult
    {
        public List<ServicePacketEntityModel> ListPacketService { get; set; }
    }
}
