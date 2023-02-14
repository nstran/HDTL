using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.MobileAppConfiguration
{
    public class CreateOrEditAdvertisementConfigurationParameter : BaseParameter
    {
       public AdvertisementConfigurationEntityModel AdvertisementConfigurationEntityModel { get; set; }
    }
}
