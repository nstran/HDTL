using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.QuyTrinh;
using TN.TNM.DataAccess.Models.ServicePacketImage;
using TN.TNM.DataAccess.Messages.Parameters.File;


namespace TN.TNM.DataAccess.Messages.Parameters.Admin.Product
{
    public class CreateOrUpdateServicePacketParameter : BaseParameter
    {
        public ServicePacketEntityModel ServicePacketEntityModel { get;set;}

        public List<ServicePacketAttributeEntityModel> ListServicePacketAttributeEntityModel {get;set;}

        public List<PermissionConfigurationEntityModel> ListServicePacketConfigurationPermissionModel { get; set; }

        public ServicePacketImageEntityModel ServicePacketImageEntityModel { get; set; }

        public List<NotificationConfigurationEntityModel> ListNotificationConfigurationModel { get; set; }

        public CauHinhQuyTrinhModel CauHinhQuyTrinh { get; set; }

        public List<Guid> ListManagerId { get; set; }
    }
}
