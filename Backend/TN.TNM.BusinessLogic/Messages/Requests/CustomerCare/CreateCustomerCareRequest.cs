using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class CreateCustomerCareRequest : BaseRequest<CreateCustomerCareParameter>
    {
        public CustomerCareEntityModel CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public List<string> ListTypeCustomer { get; set; }
        public string QueryFilter { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<int> ListSelectedTinhTrangEmail { get; set; }

        public override CreateCustomerCareParameter ToParameter()
        {
            return new CreateCustomerCareParameter
            {
                CustomerCare = CustomerCare,
                CustomerId = CustomerId,
                ListTypeCustomer = ListTypeCustomer,
                QueryFilter = QueryFilter,
                ListFormFile = ListFormFile,
                ListSelectedTinhTrangEmail = ListSelectedTinhTrangEmail,
                UserId = UserId
            };
        }
    }
}
