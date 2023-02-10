using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class RatingStatisticStarServicePacketModel
    {
        public int RateStar { get; set; }

        public int Count { get; set; }

        public string ServicePacketName { get; set; }
    }
}
