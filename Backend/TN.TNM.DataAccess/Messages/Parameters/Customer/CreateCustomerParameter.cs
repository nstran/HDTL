using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.DataAccess.Messages.Parameters.Customer
{
    public class CreateCustomerParameter : BaseParameter
    {
        public CustomerEntityModel Customer { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<ContactEntityModel> CustomerContactList { get; set; }
        public bool CreateByLead { get; set; }
        public bool IsFromLead { get; set; } // biến kiểm tra khách hàng định danh hay là khách hàng tiềm năng
        public UserEntityModel User { get; set; }
    }
    
}
