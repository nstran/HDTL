using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.QuyTrinh;
using TN.TNM.DataAccess.Models.ServicePacketImage;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetServicePacketByIdResult : BaseResult
    {
        public ServicePacketEntityModel ServicePacketEntityModel { get; set; }
        public ServicePacketImageEntityModel ServicePacketImageEntityModel { get; set; }
        public CauHinhQuyTrinhModel CauHinhQuyTrinh { get; set; }
        public List<Guid> ListManager { get; set; }
    }
}
