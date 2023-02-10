using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class CheckDuplicateCustomerAllTypeResult: BaseResult
    {
        public bool IsDuplicateLead { get; set; }
        public bool IsDuplicateCustomer { get; set; }
        public LeadEntityModel DuplicateLeadModel { get; set; }
        public ContactEntityModel DuplicateLeadContactModel { get; set; }
        public ContactEntityModel DuplicateCustomerContactModel { get; set; }
    }
}
