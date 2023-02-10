using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class SearchCustomerParameter : BaseParameter
    {
        public List<Guid> ListProvinceId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
    }
}
