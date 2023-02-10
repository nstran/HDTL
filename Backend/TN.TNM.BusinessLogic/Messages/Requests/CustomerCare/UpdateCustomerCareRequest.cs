using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.BusinessLogic.Models.CustomerCare;
using TN.TNM.DataAccess.Messages.Parameters.CustomerCare;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.BusinessLogic.Messages.Requests.CustomerCare
{
    public class UpdateCustomerCareRequest : BaseRequest<UpdateCustomerCareParameter>
    {
        public CustomerCareEntityModel CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public string QueryFilter { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<int> ListSelectedTinhTrangEmail { get; set; }

        public override UpdateCustomerCareParameter ToParameter()
        {
            return new UpdateCustomerCareParameter
            {
                CustomerCare=this.CustomerCare,
                CustomerId=this.CustomerId,
                QueryFilter=this.QueryFilter,
                ListFormFile = ListFormFile,
                ListSelectedTinhTrangEmail = ListSelectedTinhTrangEmail,
                UserId = this.UserId
            };
        }
    }
}
