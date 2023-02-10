using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeRatingStatistictDashboardResult : BaseResult
    {
        public List<RatingStatisticServicePacketModel> ListRatingStatisticServicePacketModel { get; set; }

        public List<RatingStatisticServicePacketModel> ListRatingStatisticServicePacketModelByServicePacket { get; set; }

        public List<RatingStatisticStarServicePacketModel> ListRatingStatisticStarServicePacketModel { get; set; }
    }
}
