using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Manufacture;

namespace TN.TNM.BusinessLogic.Messages.Requests.Manufacture
{
    public class SearchQuanlityControlReportRequest : BaseRequest<SearchQuanlityControlReportParameter>
    {
        public List<Guid?> ListTechniqueRequestId { get; set; }
        public List<int?> ListThicknessOptionId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public override SearchQuanlityControlReportParameter ToParameter()
        {
            return new SearchQuanlityControlReportParameter()
            {
                ListTechniqueRequestId = ListTechniqueRequestId ?? new List<Guid?>(),
                ListThicknessOptionId = ListThicknessOptionId ?? new List<int?>(),
                FromDate = FromDate,
                ToDate = ToDate,
                UserId = UserId
            };
        }
    }
}
