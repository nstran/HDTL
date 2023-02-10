using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Customer;
using TN.TNM.DataAccess.Messages.Parameters.Lead;

namespace TN.TNM.BusinessLogic.Messages.Requests.Customer
{
    public class CreatePotentialCustomerFromWebRequest : BaseRequest<CreatePotentialCutomerFromWebParameter>
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Deputy { get; set; }
        public string Description { get; set; }
        public string BusinessScale { get; set; }//Quy mô doanh nghiệp(nhỏ,vừa)
        public string TokenId { get; set; }
        public string Role { get; set; }
        public string TenantHost { get; set; }
        public List<string> ListProductCode { get; set; }
        public override CreatePotentialCutomerFromWebParameter ToParameter()
        {
            return new CreatePotentialCutomerFromWebParameter
            {
                CustomerName = this.CustomerName,
                Address = this.Address,
                BusinessScale = this.BusinessScale,
                Email = this.Email,
                Deputy = this.Deputy,
                Phone = this.Phone,
                Role = this.Role,
                Description = this.Description,
                ListProductCode = this.ListProductCode,
                TenantHost = this.TenantHost,
                TokenId = this.TokenId
            };
        }
    }
}
