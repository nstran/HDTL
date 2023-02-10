using System.Collections.Generic;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductCategory;
using System;
using System.Text;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.Options;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Product
{
    public class GetMasterDataCreateServicePacketResult: BaseResult
    {
        public List<TrangThaiGeneral> ListDataTypeAttr { get; set; }
        public List<RoleEntityModel> ListRoleServicePacket { get; set; }
        public List<CategoryEntityModel> ListAttrCategory { get; set; }
        public List<ProductCategoryEntityModel> ListProductCategoryEntityModel { get; set; }
        public List<EmployeeEntityModel> ListEmpWithRole { get; set; }
        public List<OptionsEntityModel> ListOptionEntityModel { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public List<PermissionConfigurationEntityModel> ListStepServicePacketSelect { get; set; }
        public List<PermissionConfigurationEntityModel> ListServicePacketConfigurationPermissionModel { get; set; }
        public List<NotificationConfigurationEntityModel> ListNotificationConfigurationEntityModel { get; set; }

    }
}
