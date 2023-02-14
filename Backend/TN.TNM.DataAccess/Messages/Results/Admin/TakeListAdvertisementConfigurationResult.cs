using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Admin
{
    public class TakeListAdvertisementConfigurationResult : BaseResult
    {
        public List<AdvertisementConfigurationEntityModel> ListAdvertisementConfigurationEntityModel { get; set; }
    }
}
