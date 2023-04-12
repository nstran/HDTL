using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Entities
{
    public class AdvertisementConfigurationEntityModel
    {
        public Guid? Id { get; set; }
        public string Image { get; set; }
        public string ImageName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? SortOrder { get; set; }
        public bool Edit { get; set; }
    }
}
