using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;
using TN.TNM.DataAccess.Models.Options;
using TN.TNM.DataAccess.Models.Product;

namespace TN.TNM.DataAccess.Messages.Results.Order
{
    public class SearchOptionOfPacketServiceResult: BaseResult
    {
        public List<AttributeConfigurationEntityModel> ListAttrPacket { get; set; }
        public List<AttributeConfigurationEntityModel> ListOptionAttr { get; set; }
        public List<ServicePacketMappingOptionsEntityModel> ListOption { get; set; }
        public List<TrangThaiGeneral> ListDataType { get; set; }
    }
}
