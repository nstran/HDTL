using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.AttributeConfigurationEntityModel;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.CustomerOrder;
using TN.TNM.DataAccess.Models.MilestoneConfiguration;
using TN.TNM.DataAccess.Models.Options;
using TN.TNM.DataAccess.Models.Order;
using TN.TNM.DataAccess.Models.PermissionConfiguration;
using TN.TNM.DataAccess.Models.Product;
using TN.TNM.DataAccess.Models.ProductMappingOptions;
using TN.TNM.DataAccess.Models.ServicePacketImage;
using TN.TNM.DataAccess.Models.Entities;

namespace TN.TNM.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Options, OptionsEntityModel>().ReverseMap();
            CreateMap<MilestoneConfiguration, MilestoneConfigurationEntityModel>().ReverseMap();
            CreateMap<Product, ProductEntityModel>().ReverseMap();
            CreateMap<ProductMappingOptions, ProductMappingOptionModel>().ReverseMap();
            CreateMap<AttributeConfiguration, AttributeConfigurationEntityModel>().ReverseMap();
            CreateMap<ServicePacketEntityModel, ServicePacket>();
            CreateMap<PermissionConfiguration, PermissionConfigurationEntityModel>().ReverseMap();
            CreateMap<ServicePacketImage, ServicePacketImageEntityModel>().ReverseMap();
            CreateMap<CustomerOrderDetail, CustomerOrderDetailEntityModel>().ReverseMap();
            CreateMap<CustomerOrderExtension, CustomerOrderExtensionEntityModel>().ReverseMap();
            CreateMap<Employee, EmployeeEntityModel>().ReverseMap();
            CreateMap<Organization, OrganizationEntityModel>().ReverseMap();
            CreateMap<CustomerOrderDetailExten, CustomerOrderDetailExtenEntityModel>().ReverseMap();
            CreateMap<ServicePacketMappingOptions, ServicePacketMappingOptionsEntityModel>().ReverseMap();
            CreateMap<CustomerOrderTask, CustomerOrderTaskEntityModel>().ReverseMap();
            CreateMap<ReportPoint, ReportPointEntityModel>().ReverseMap();
            CreateMap<OrderProcess, OrderProcessEntityModel > ().ReverseMap();
            CreateMap<MobileAppConfiguration, MobileAppConfigurationEntityModel>().ReverseMap();
            CreateMap<NotificationConfiguration, NotificationConfigurationEntityModel>().ReverseMap();
            CreateMap<PaymentMethodConfigure, PaymentMethodConfigureEntityModel>().ReverseMap();
            CreateMap<AdvertisementConfiguration, AdvertisementConfigurationEntityModel>().ReverseMap();
        }
    }
}