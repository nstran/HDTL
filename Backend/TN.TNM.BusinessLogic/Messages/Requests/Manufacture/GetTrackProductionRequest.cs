using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class GetTrackProductionRequest : BaseRequest<GetTrackProductionParameter>
    {
        public List<Guid> ListProductionOrderId { get; set; }
        public DateTime? EndDate { get; set; }
        public double? ProductThickness { get; set; }
        public string ProductName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<Guid> ListStatusItem { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public int FirstNumber { get; set; }
        public int Rows { get; set; }

        public override GetTrackProductionParameter ToParameter()
        {
            return new GetTrackProductionParameter()
            {
                UserId = UserId,
                ListProductionOrderId = ListProductionOrderId,
                EndDate = EndDate,
                ProductThickness = ProductThickness,
                ProductName = ProductName,
                FromDate = FromDate,
                ToDate = ToDate,
                ListStatusItem = ListStatusItem,
                ProductLength = ProductLength,
                ProductWidth = ProductWidth,
                FirstNumber = FirstNumber,
                Rows = Rows
            };
        }
    }
}
