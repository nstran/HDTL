using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.ServicePacketImage;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ServicePacketEntityModel
    {
        public  Guid?  Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string AttributeName { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string MainImage { get; set; }
        public string Icon { get; set; }
        public string BackgroundImage { get; set; }
        public int? CountOption { get; set; }
        public int? Stt { get; set; }
        public List<PermissionConfigurationEntityModel> ListPermissionConfigurationEntityModel { get; set; }
        public List<RoleEntityModel> ListRoleServicePacket { get; set; }
        public List<ServicePacketAttributeEntityModel> ListServicePacketAttributeEntityModel { get; set; }
        public List<NotificationConfigurationEntityModel> ListNotificationConfigurationEntityModel { get; set; }
    }
}
