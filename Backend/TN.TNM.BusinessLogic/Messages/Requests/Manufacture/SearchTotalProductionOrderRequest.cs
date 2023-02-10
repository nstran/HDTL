using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class SearchTotalProductionOrderRequest : BaseRequest<SearchTotalProductionOrderParameter>
    {
        public string Code { get; set; }
        public DateTime? StartDate { get; set; }
        public double? TotalQuantity { get; set; }
        public double? TotalArea { get; set; }
        public DateTime? MinEndDate { get; set; }
        public DateTime? MaxEndDate { get; set; }
        public List<Guid> ListStatusId { get; set; }
        public int FirstNumber { get; set; }
        public int Rows { get; set; }

        public override SearchTotalProductionOrderParameter ToParameter()
        {
            return new SearchTotalProductionOrderParameter()
            {
                UserId = UserId,
                Code = Code,
                StartDate = StartDate,
                TotalQuantity = TotalQuantity,
                TotalArea = TotalArea,
                MinEndDate = MinEndDate,
                MaxEndDate = MaxEndDate,
                ListStatusId = ListStatusId,
                FirstNumber = FirstNumber,
                Rows = Rows
            };
        }
    }
}
