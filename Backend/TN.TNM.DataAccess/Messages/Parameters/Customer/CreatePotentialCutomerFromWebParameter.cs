using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreatePotentialCutomerFromWebParameter : BaseParameter
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Deputy { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string BusinessScale { get; set; }//Quy mô doanh nghiệp(nhỏ,vừa)
        public string TenantHost { get; set; }
        public List<string> ListProductCode { get; set; }
        public string TokenId { get; set; }
    }
}
