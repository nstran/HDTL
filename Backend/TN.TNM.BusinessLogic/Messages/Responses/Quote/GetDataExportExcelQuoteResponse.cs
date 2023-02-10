using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Category;
using TN.TNM.BusinessLogic.Models.Employee;
using TN.TNM.BusinessLogic.Models.Quote;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetDataExportExcelQuoteResponse : BaseResponse
    {
        public InforExportExcelFactoryModel InforExportExcel { get; set; }

        public QuoteModel Quote { get; set; }

        public List<QuoteDetailModel> ListQuoteDetail { get; set; }

        public List<AdditionalInformationModel> ListAdditionalInformation { get; set; }

        public List<QuotePlanModel> ListQuotePlan { get; set; }

        public List<QuotePaymentTermModel> ListQuotePaymentTerm { get; set; }

        public List<QuoteCostDetailEntityModel> ListQuoteCostDetail { get; set; }
    }
}
