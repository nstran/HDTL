using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Admin.Dashboard
{
    public class TakeStatisticServiceTicketDashboardResult : BaseResult
    {
        public List<NewStatus> ListNewStatus { get; set; }

        public List<ProgressStatus> ListProgressStatus { get; set; }

        public List<DoneStatus> ListDoneStatus { get; set; }

        public List<CancelStatus> ListCancelStatus { get; set; }

    }

    public class NewStatus
    {
        public string ServicePacketName { get; set; }

        public string ProductCategoryName { get; set; }

        public int? Count { get; set; }

        public List<NewStatus> ListStatisticServicePacketNewStatus { get; set; }
    }

    public class ProgressStatus
    {
        public string ServicePacketName { get; set; }

        public string ProductCategoryName { get; set; }

        public int? Count { get; set; }

        public List<ProgressStatus> ListStatisticServicePacketProgressStatus { get; set; }
    }

    public class DoneStatus
    {
        public string ServicePacketName { get; set; }

        public string ProductCategoryName { get; set; }

        public int? Count { get; set; }

        public List<DoneStatus> ListStatisticServicePacketDoneStatus { get; set; }
    }

    public class CancelStatus
    {
        public string ServicePacketName { get; set; }

        public string ProductCategoryName { get; set; }

        public int? Count { get; set; }

        public List<CancelStatus> ListStatisticServicePacketCancelStatus { get; set; }
    }
}
