using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class RatingStatisticServicePacketModel
    {
        public string ProductCategoryName { get; set; }

        public List<int> ListRate { get; set; }

        public string ServicePacketName { get; set; }
    }
}
