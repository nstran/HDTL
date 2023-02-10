using System.Collections.Generic;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class FilterCustomerRequest : BaseRequest<FilterCustomerParameter>
    {
        public string SqlQuery { get; set; }
        public List<string> TypeCustomer { get; set; }
        public bool? IsSendEmail { get; set; }
        public List<int> ListSelectedTinhTrangEmail { get; set; }

        public override FilterCustomerParameter ToParameter()
        {
            return new FilterCustomerParameter
            {
                SqlQuery = this.SqlQuery,
                CustomerStatusCode = TypeCustomer,
                IsSendEmail = IsSendEmail,
                ListSelectedTinhTrangEmail = ListSelectedTinhTrangEmail,
                UserId = this.UserId
            };
        }
    }
}
