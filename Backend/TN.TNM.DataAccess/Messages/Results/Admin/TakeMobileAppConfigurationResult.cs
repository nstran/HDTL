using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class TakeMobileAppConfigurationResult : BaseResult
    {
        public MobileAppConfigurationEntityModel MobileAppConfigurationEntityModel { get; set; }

        public List<CategoryEntityModel> ListPayMentCategory { get; set; }

        public List<PaymentMethodConfigureEntityModel> ListPayMent { get; set; }

        public List<AdvertisementConfigurationEntityModel> ListAdvertisementConfigurationEntityModel { get; set; }
    }
}
