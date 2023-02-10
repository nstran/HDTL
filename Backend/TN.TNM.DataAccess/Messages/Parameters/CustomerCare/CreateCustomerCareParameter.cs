using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TN.TNM.DataAccess.Models.CustomerCare;

namespace TN.TNM.DataAccess.Messages.Parameters.CustomerCare
{
    public class CreateCustomerCareParameter : BaseParameter
    {
        public CustomerCareEntityModel CustomerCare { get; set; }
        public List<Guid> CustomerId { get; set; }
        public List<string> ListTypeCustomer { get; set; }
        public string QueryFilter { get; set; }
        public List<IFormFile> ListFormFile { get; set; }
        public List<int> ListSelectedTinhTrangEmail { get; set; }
    }
}
